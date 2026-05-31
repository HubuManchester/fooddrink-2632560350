using Microsoft.Maui.Devices;

namespace FlavorVault.Services;

/// <summary>
/// 振动反馈服务，提供三级振动反馈
/// 使用 Microsoft.Maui.Devices.Vibration 或平台原生实现
/// </summary>
public class HapticService
{
    /// <summary>
    /// 成功反馈：100ms 中等振动
    /// </summary>
    public void PerformSuccess()
    {
        try
        {
#if ANDROID
            PerformAndroidVibration(100);
#else
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
#endif
        }
        catch (FeatureNotSupportedException)
        {
            // 平台不支持振动
        }
        catch (Exception)
        {
            // 静默处理
        }
    }

    /// <summary>
    /// 错误反馈：双振动（100ms + 暂停100ms + 100ms）
    /// </summary>
    public void PerformError()
    {
        try
        {
#if ANDROID
            PerformAndroidDoubleVibration(100, 100, 100);
#else
            // 非 Android 平台使用简单振动模拟
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(300));
#endif
        }
        catch (FeatureNotSupportedException)
        {
            // 平台不支持振动
        }
        catch (Exception)
        {
            // 静默处理
        }
    }

    /// <summary>
    /// 轻量反馈：50ms 轻振动
    /// </summary>
    public void PerformLight()
    {
        try
        {
#if ANDROID
            PerformAndroidVibration(50);
#else
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(50));
#endif
        }
        catch (FeatureNotSupportedException)
        {
            // 平台不支持振动
        }
        catch (Exception)
        {
            // 静默处理
        }
    }

    /// <summary>
    /// 异步成功反馈
    /// </summary>
    public Task SuccessAsync()
    {
        PerformSuccess();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 异步错误反馈
    /// </summary>
    public Task ErrorAsync()
    {
        PerformError();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 异步轻量反馈
    /// </summary>
    public Task LightAsync()
    {
        PerformLight();
        return Task.CompletedTask;
    }

#if ANDROID
    /// <summary>
    /// Android 平台精确振动控制
    /// </summary>
#pragma warning disable CA1416, CA1422
    private void PerformAndroidVibration(int durationMs)
    {
        try
        {
            var context = Platform.AppContext;
            if (context == null) return;

            var vibrator = (Android.OS.Vibrator?)context.GetSystemService(Android.Content.Context.VibratorService);
            if (vibrator == null) return;

            if (vibrator.HasVibrator)
            {
                vibrator.Vibrate(Android.OS.VibrationEffect.CreateOneShot(
                    (long)durationMs,
                    Android.OS.VibrationEffect.DefaultAmplitude));
            }
        }
        catch (Exception)
        {
            // 降级使用 MAUI API
            try
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(durationMs));
            }
            catch
            {
                // 最终降级，完全静默
            }
        }
    }

    /// <summary>
    /// Android 平台双振动模式
    /// </summary>
    private void PerformAndroidDoubleVibration(int firstMs, int pauseMs, int secondMs)
    {
        try
        {
            var context = Platform.AppContext;
            if (context == null) return;

            var vibrator = (Android.OS.Vibrator?)context.GetSystemService(Android.Content.Context.VibratorService);
            if (vibrator == null) return;

            if (vibrator.HasVibrator)
            {
                // 构建振动模式：延迟0开始，振动 firstMs，暂停 pauseMs，振动 secondMs
                var timings = new long[] { 0, firstMs, pauseMs, secondMs };
                var amplitudes = new int[] { 0, Android.OS.VibrationEffect.DefaultAmplitude, 0, Android.OS.VibrationEffect.DefaultAmplitude };
                var effect = Android.OS.VibrationEffect.CreateWaveform(timings, amplitudes, -1);
                vibrator.Vibrate(effect);
            }
        }
        catch (Exception)
        {
            // 降级：使用简单连续振动
            try
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(firstMs + pauseMs + secondMs));
            }
            catch
            {
                // 最终降级
            }
        }
    }
#pragma warning restore CA1416, CA1422
#endif
}
