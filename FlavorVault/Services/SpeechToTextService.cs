using Microsoft.Maui.ApplicationModel;

namespace FlavorVault.Services;

public class SpeechToTextService
{
    private bool _isListening;
    private string _language = "en-US";

    public bool IsListening => _isListening;

    public string Language
    {
        get => _language;
        set => _language = value;
    }

    public event Action<string>? OnRecognitionResult;

    public async Task<string> ListenAsync(CancellationToken cancellationToken = default)
    {
        if (_isListening) return string.Empty;

        _isListening = true;
        try
        {
            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted) return string.Empty;

            return await RecognizeInternalAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SpeechToText] ListenAsync 错误: {ex.Message}");
            return string.Empty;
        }
        finally
        {
            _isListening = false;
        }
    }

    public Task<string> RecognizeAsync(CancellationToken cancellationToken = default)
    {
        return ListenAsync(cancellationToken);
    }

#if WINDOWS
    private async Task<string> RecognizeInternalAsync(CancellationToken ct)
    {
        try
        {
            var lang = _language == "zh-CN" ? "zh-CN" : "en-US";
            var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer(
                new Windows.Globalization.Language(lang));

            await speechRecognizer.CompileConstraintsAsync();

            speechRecognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(2);
            speechRecognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(8);
            speechRecognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(5);

            var result = await speechRecognizer.RecognizeAsync();

            speechRecognizer.Dispose();

            if (result.Status == Windows.Media.SpeechRecognition.SpeechRecognitionResultStatus.Success)
                return result.Text;

            return string.Empty;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SpeechToText] Windows 错误: {ex.Message}");
            return string.Empty;
        }
    }
#elif ANDROID
    private async Task<string> RecognizeInternalAsync(CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<string>();

        var lang = _language;

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                var intent = new Android.Content.Intent(Android.Speech.RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(Android.Speech.RecognizerIntent.ExtraLanguageModel, Android.Speech.RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(Android.Speech.RecognizerIntent.ExtraLanguage, lang);
                intent.PutExtra(Android.Speech.RecognizerIntent.ExtraPrompt, "请说出内容");
                intent.PutExtra(Android.Speech.RecognizerIntent.ExtraMaxResults, 1);

                var activity = Platform.CurrentActivity;
                if (activity == null)
                {
                    tcs.TrySetResult(string.Empty);
                    return;
                }

                if (!activity.PackageManager!.HasSystemFeature("android.hardware.microphone"))
                {
                    tcs.TrySetResult(string.Empty);
                    return;
                }

                activity.StartActivityForResult(intent, 1001);

                tcs.TrySetResult(string.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SpeechToText] Android 错误: {ex.Message}");
                tcs.TrySetResult(string.Empty);
            }
        });

        return await tcs.Task;
    }
#else
    private Task<string> RecognizeInternalAsync(CancellationToken ct)
    {
        return Task.FromResult(string.Empty);
    }
#endif
}
