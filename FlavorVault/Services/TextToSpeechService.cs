using Microsoft.Maui.Media;

namespace FlavorVault.Services;

/// <summary>
/// 文字转语音服务，使用 Microsoft.Maui.Media.TextToSpeech
/// 单 CancellationTokenSource 管理朗读生命周期，同一时间只有一个朗读任务
/// </summary>
public class TextToSpeechService
{
    private CancellationTokenSource? _cts;

    /// <summary>
    /// 当前是否正在朗读
    /// </summary>
    public bool IsSpeaking => _cts != null && !_cts.IsCancellationRequested;

    /// <summary>
    /// 朗读文本
    /// </summary>
    /// <param name="text">要朗读的文本</param>
    public async Task SpeakAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        try
        {
            // 每次朗读前先停止之前的任务，确保同一时间只有一个朗读
            StopSpeaking();

            _cts = new CancellationTokenSource();

            // 尝试获取中文 Locale
            var locale = await GetChineseLocaleAsync();

            await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions
            {
                Locale = locale
            }, _cts.Token);
        }
        catch (OperationCanceledException)
        {
            // 朗读被取消是正常行为，不做处理
        }
        catch (FeatureNotSupportedException)
        {
            // 当前平台不支持 TTS
        }
        catch (Exception)
        {
            // 其他异常静默处理，防止崩溃
        }
    }

    /// <summary>
    /// 停止当前朗读
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
    /// 异步停止朗读
    /// </summary>
    public Task CancelAsync()
    {
        StopSpeaking();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 切换朗读/停止状态
    /// </summary>
    /// <param name="text">要朗读的文本</param>
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
            // 静默处理，防止崩溃
        }
    }

    /// <summary>
    /// 获取中文 Locale，若无则返回默认
    /// </summary>
    private async Task<Locale?> GetChineseLocaleAsync()
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            // 优先匹配中文 Locale
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
