using Microsoft.Maui.Devices.Sensors;

namespace FlavorVault.Services;

/// <summary>
/// 传感器服务，包含摇一摇检测和指南针方向
/// 使用 Microsoft.Maui.Devices.Sensors
/// </summary>
public class SensorService
{
    /// <summary>
    /// 摇一摇检测事件
    /// </summary>
    public event EventHandler? ShakeDetected;

    /// <summary>
    /// 指南针方向变更事件
    /// </summary>
    public event EventHandler<double>? CompassChanged;

    #region 摇一摇检测

    private const double ShakeThreshold = 2.5; // 加速度阈值约 2.5g
    private const int ShakeMinIntervalMs = 500; // 最小触发间隔 500ms
    private DateTime _lastShakeTime = DateTime.MinValue;
    private Action? _onShakeCallback;
    private bool _isShakeDetectionActive;
    private double _lastX, _lastY, _lastZ;

    /// <summary>
    /// 开始摇一摇检测
    /// </summary>
    /// <param name="onShake">检测到摇动时的回调</param>
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

            // 记录初始值
            if (Accelerometer.Default.IsMonitoring)
            {
                Accelerometer.Default.Stop();
            }

            Accelerometer.Default.ReadingChanged += OnAccelerometerReadingChanged;
            Accelerometer.Default.Start(SensorSpeed.Game);
        }
        catch (FeatureNotSupportedException)
        {
            // 设备不支持加速度计
        }
        catch (Exception)
        {
            // 静默处理
        }
    }

    /// <summary>
    /// 停止摇一摇检测
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
            // 静默处理
        }
    }

    private void OnAccelerometerReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        try
        {
            var data = e.Reading;

            // 计算加速度变化量
            var deltaX = Math.Abs(data.Acceleration.X - _lastX);
            var deltaY = Math.Abs(data.Acceleration.Y - _lastY);
            var deltaZ = Math.Abs(data.Acceleration.Z - _lastZ);

            // 计算总加速度变化
            var totalDelta = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

            // 更新上次值
            _lastX = data.Acceleration.X;
            _lastY = data.Acceleration.Y;
            _lastZ = data.Acceleration.Z;

            // 检查是否超过阈值，且满足最小间隔
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
            // 静默处理
        }
    }

    /// <summary>
    /// 开始摇一摇检测（无参数版本，使用事件）
    /// </summary>
    public void StartShakeDetection()
    {
        StartShakeDetection(() =>
        {
            ShakeDetected?.Invoke(this, EventArgs.Empty);
        });
    }

    #endregion

    #region 指南针

    private Action<double>? _onHeadingChanged;
    private bool _isCompassActive;

    /// <summary>
    /// 开始监听指南针方向变化
    /// </summary>
    /// <param name="onHeadingChanged">方向变化回调，参数为磁北方向角（0-360度）</param>
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
            // 设备不支持指南针
        }
        catch (Exception)
        {
            // 静默处理
        }
    }

    /// <summary>
    /// 停止指南针监听
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
            // 静默处理
        }
    }

    private void OnCompassReadingChanged(object? sender, CompassChangedEventArgs e)
    {
        try
        {
            // 获取磁北方向角，范围 0-360
            var heading = e.Reading.HeadingMagneticNorth;

            // 确保角度在 0-360 范围内
            heading = ((heading % 360) + 360) % 360;

            _onHeadingChanged?.Invoke(heading);
        }
        catch (Exception)
        {
            // 静默处理
        }
    }

    /// <summary>
    /// 开始指南针（无参数版本，使用事件）
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
