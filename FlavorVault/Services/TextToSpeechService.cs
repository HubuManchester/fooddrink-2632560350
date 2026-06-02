using Microsoft.Maui.Media;

namespace FlavorVault.Services;

/// <summary>
/// Text-to-speech service using Microsoft.Maui.Media.TextToSpeech
/// Manages speech lifecycle with a single CancellationTokenSource, only one speech task at a time
/// </summary>
public class TextToSpeechService
{
    private CancellationTokenSource? _cts;

    /// <summary>
    /// Whether speech is currently playing
    /// </summary>
    public bool IsSpeaking => _cts != null && !_cts.IsCancellationRequested;

    /// <summary>
    /// Speak text aloud
    /// </summary>
    /// <param name="text">The text to speak</param>
    public async Task SpeakAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        try
        {
            // Stop any previous speech before starting a new one, ensuring only one speech at a time
            StopSpeaking();

            _cts = new CancellationTokenSource();

            // Try to get a Chinese Locale
            var locale = await GetChineseLocaleAsync();

            await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions
            {
                Locale = locale
            }, _cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Speech cancellation is normal behavior, no handling needed
        }
        catch (FeatureNotSupportedException)
        {
            // Current platform does not support TTS
        }
        catch (Exception)
        {
            // Silently handle other exceptions to prevent crashes
        }
    }

    /// <summary>
    /// Stop the current speech
    /// </summary>
    public void StopSpeaking()
    {
        try
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
        }
    }

    /// <summary>
    /// Async stop speech
    /// </summary>
    public Task CancelAsync()
    {
        StopSpeaking();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Toggle speech play/stop state
    /// </summary>
    /// <param name="text">The text to speak</param>
    public async void ToggleSpeaking(string text)
    {
        try
        {
            if (IsSpeaking)
            {
                StopSpeaking();
            }
            else
            {
                await SpeakAsync(text);
            }
        }
        catch (Exception)
        {
            // Silent handling to prevent crashes
        }
    }

    /// <summary>
    /// Get a Chinese Locale, or return default if not available
    /// </summary>
    private async Task<Locale?> GetChineseLocaleAsync()
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            // Prefer matching a Chinese Locale
            var chineseLocale = locales.FirstOrDefault(l =>
                l.Language != null &&
                (l.Language.StartsWith("zh", StringComparison.OrdinalIgnoreCase) ||
                 l.Language.StartsWith("cmn", StringComparison.OrdinalIgnoreCase)));

            return chineseLocale;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
