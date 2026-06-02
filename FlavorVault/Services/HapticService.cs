using Microsoft.Maui.Devices;

namespace FlavorVault.Services;

/// <summary>
/// Haptic feedback service providing three levels of vibration feedback
/// Uses Microsoft.Maui.Devices.Vibration or platform-native implementation
/// </summary>
public class HapticService
{
    /// <summary>
    /// Success feedback: 100ms medium vibration
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
            // Platform does not support vibration
        }
        catch (Exception)
        {
            // Silent handling
        }
    }

    /// <summary>
    /// Error feedback: double vibration (100ms + pause 100ms + 100ms)
    /// </summary>
    public void PerformError()
    {
        try
        {
#if ANDROID
            PerformAndroidDoubleVibration(100, 100, 100);
#else
            // Non-Android platforms use simple vibration simulation
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(300));
#endif
        }
        catch (FeatureNotSupportedException)
        {
            // Platform does not support vibration
        }
        catch (Exception)
        {
            // Silent handling
        }
    }

    /// <summary>
    /// Light feedback: 50ms light vibration
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
            // Platform does not support vibration
        }
        catch (Exception)
        {
            // Silent handling
        }
    }

    /// <summary>
    /// Async success feedback
    /// </summary>
    public Task SuccessAsync()
    {
        PerformSuccess();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Async error feedback
    /// </summary>
    public Task ErrorAsync()
    {
        PerformError();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Async light feedback
    /// </summary>
    public Task LightAsync()
    {
        PerformLight();
        return Task.CompletedTask;
    }

#if ANDROID
    /// <summary>
    /// Android platform precise vibration control
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
            // Fallback to MAUI API
            try
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(durationMs));
            }
            catch
            {
                // Final fallback, completely silent
            }
        }
    }

    /// <summary>
    /// Android platform double vibration pattern
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
                // Build vibration pattern: start at delay 0, vibrate firstMs, pause pauseMs, vibrate secondMs
                var timings = new long[] { 0, firstMs, pauseMs, secondMs };
                var amplitudes = new int[] { 0, Android.OS.VibrationEffect.DefaultAmplitude, 0, Android.OS.VibrationEffect.DefaultAmplitude };
                var effect = Android.OS.VibrationEffect.CreateWaveform(timings, amplitudes, -1);
                vibrator.Vibrate(effect);
            }
        }
        catch (Exception)
        {
            // Fallback: use simple continuous vibration
            try
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(firstMs + pauseMs + secondMs));
            }
            catch
            {
                // Final fallback
            }
        }
    }
#pragma warning restore CA1416, CA1422
#endif
}
