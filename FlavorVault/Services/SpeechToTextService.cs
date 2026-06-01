using Microsoft.Maui.ApplicationModel;

namespace FlavorVault.Services;

/// <summary>
/// 语音转文字服务（占位实现）
/// MAUI 当前版本无内置语音识别，此处提供接口兼容
/// </summary>
public class SpeechToTextService
{
    private bool _isListening;

    public bool IsListening => _isListening;

    public event Action<string>? OnRecognitionResult;

    public async Task<string> ListenAsync(CancellationToken cancellationToken = default)
    {
        if (_isListening) return string.Empty;

        _isListening = true;
        try
        {
            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted) return string.Empty;

            // 当前 MAUI 版本无内置语音识别
            // 实际项目中可集成第三方语音识别 SDK
            return string.Empty;
        }
        catch (Exception)
        {
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
}
