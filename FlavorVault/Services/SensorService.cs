using Microsoft.Maui.Devices.Sensors;

namespace FlavorVault.Services;

/// <summary>
/// Sensor service, including shake detection and compass direction
/// Uses Microsoft.Maui.Devices.Sensors
/// </summary>
public class SensorService
{
    /// <summary>
    /// Shake detection event
    /// </summary>
    public event EventHandler? ShakeDetected;

    /// <summary>
    /// Compass direction change event
    /// </summary>
    public event EventHandler<double>? CompassChanged;

    #region Shake Detection

    private const double ShakeThreshold = 2.5; // Acceleration threshold ~2.5g
    private const int ShakeMinIntervalMs = 500; // Minimum trigger interval 500ms
    private DateTime _lastShakeTime = DateTime.MinValue;
    private Action? _onShakeCallback;
    private bool _isShakeDetectionActive;
    private double _lastX, _lastY, _lastZ;

    /// <summary>
    /// Start shake detection
    /// </summary>
    /// <param name="onShake">Callback when shake is detected</param>
    public void StartShakeDetection(Action onShake)
    {
        try
        {
            if (_isShakeDetectionActive)
                return;

            if (!Accelerometer.Default.IsSupported)
                return;

            _onShakeCallback = onShake;
            _isShakeDetectionActive = true;

            // Record initial values
            if (Accelerometer.Default.IsMonitoring)
            {
                Accelerometer.Default.Stop();
            }

            Accelerometer.Default.ReadingChanged += OnAccelerometerReadingChanged;
            Accelerometer.Default.Start(SensorSpeed.Game);
        }
        catch (FeatureNotSupportedException)
        {
            // Device does not support accelerometer
        }
        catch (Exception)
        {
            // Silent handling
        }
    }

    /// <summary>
    /// Stop shake detection
    /// </summary>
    public void StopShakeDetection()
    {
        try
        {
            if (!_isShakeDetectionActive)
                return;

            Accelerometer.Default.ReadingChanged -= OnAccelerometerReadingChanged;

            if (Accelerometer.Default.IsMonitoring)
            {
                Accelerometer.Default.Stop();
            }

            _isShakeDetectionActive = false;
            _onShakeCallback = null;
        }
        catch (Exception)
        {
            // Silent handling
        }
    }

    private void OnAccelerometerReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        try
        {
            var data = e.Reading;

            // Calculate acceleration change
            var deltaX = Math.Abs(data.Acceleration.X - _lastX);
            var deltaY = Math.Abs(data.Acceleration.Y - _lastY);
            var deltaZ = Math.Abs(data.Acceleration.Z - _lastZ);

            // Calculate total acceleration change
            var totalDelta = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

            // Update last values
            _lastX = data.Acceleration.X;
            _lastY = data.Acceleration.Y;
            _lastZ = data.Acceleration.Z;

            // Check if threshold is exceeded and minimum interval is met
            if (totalDelta > ShakeThreshold)
            {
                var now = DateTime.Now;
                if ((now - _lastShakeTime).TotalMilliseconds >= ShakeMinIntervalMs)
                {
                    _lastShakeTime = now;
                    _onShakeCallback?.Invoke();
                }
            }
        }
        catch (Exception)
        {
            // Silent handling
        }
    }

    /// <summary>
    /// Start shake detection (parameterless version, uses events)
    /// </summary>
    public void StartShakeDetection()
    {
        StartShakeDetection(() =>
        {
            ShakeDetected?.Invoke(this, EventArgs.Empty);
        });
    }

    #endregion

    #region Compass

    private Action<double>? _onHeadingChanged;
    private bool _isCompassActive;

    /// <summary>
    /// Start listening for compass direction changes
    /// </summary>
    /// <param name="onHeadingChanged">Direction change callback, parameter is magnetic north heading angle (0-360 degrees)</param>
    public void StartCompass(Action<double> onHeadingChanged)
    {
        try
        {
            if (_isCompassActive)
                return;

            if (!Compass.Default.IsSupported)
                return;

            _onHeadingChanged = onHeadingChanged;
            _isCompassActive = true;

            if (Compass.Default.IsMonitoring)
            {
                Compass.Default.Stop();
            }

            Compass.Default.ReadingChanged += OnCompassReadingChanged;
            Compass.Default.Start(SensorSpeed.UI);
        }
        catch (FeatureNotSupportedException)
        {
            // Device does not support compass
        }
        catch (Exception)
        {
            // Silent handling
        }
    }

    /// <summary>
    /// Stop compass listening
    /// </summary>
    public void StopCompass()
    {
        try
        {
            if (!_isCompassActive)
                return;

            Compass.Default.ReadingChanged -= OnCompassReadingChanged;

            if (Compass.Default.IsMonitoring)
            {
                Compass.Default.Stop();
            }

            _isCompassActive = false;
            _onHeadingChanged = null;
        }
        catch (Exception)
        {
            // Silent handling
        }
    }

    private void OnCompassReadingChanged(object? sender, CompassChangedEventArgs e)
    {
        try
        {
            // Get magnetic north heading angle, range 0-360
            var heading = e.Reading.HeadingMagneticNorth;

            // Ensure angle is within 0-360 range
            heading = ((heading % 360) + 360) % 360;

            _onHeadingChanged?.Invoke(heading);
        }
        catch (Exception)
        {
            // Silent handling
        }
    }

    /// <summary>
    /// Start compass (parameterless version, uses events)
    /// </summary>
    public void StartCompass()
    {
        StartCompass(heading =>
        {
            CompassChanged?.Invoke(this, heading);
        });
    }

    #endregion
}
