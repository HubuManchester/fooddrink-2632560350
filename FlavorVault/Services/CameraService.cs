using Microsoft.Maui.Media;
using Microsoft.Maui.ApplicationModel;

namespace FlavorVault.Services;

/// <summary>
/// 相机服务，使用 Microsoft.Maui.Media.MediaPicker
/// 包含拍照、相册选取、保存文件及 Android 闪光灯控制
/// </summary>
public class CameraService
{
#if ANDROID
    private bool _isFlashOn;
#endif

    /// <summary>
    /// 闪光灯是否开启（仅 Android 有效）
    /// </summary>
    public bool IsFlashOn
    {
        get
        {
#if ANDROID
            return _isFlashOn;
#else
            return false;
#endif
        }
        set
        {
#if ANDROID
            _isFlashOn = value;
#endif
        }
    }

    /// <summary>
    /// 拍照并返回文件结果
    /// </summary>
    /// <returns>拍照结果，失败或取消返回 null</returns>
    public async Task<FileResult?> CapturePhotoAsync()
    {
        try
        {
#if WINDOWS
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                return null;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = $"FV_{DateTime.Now:yyyyMMdd_HHmmss}.jpg"
            });
            return photo;
#else
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                return null;
            }

            if (!MediaPicker.Default.IsCaptureSupported)
            {
                return null;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = $"FV_{DateTime.Now:yyyyMMdd_HHmmss}.jpg"
            });
            return photo;
#endif
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CameraService] CapturePhotoAsync 错误: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// 从相册选取照片
    /// </summary>
    /// <returns>选取结果，取消返回 null</returns>
    public async Task<FileResult?> PickPhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "选择照片"
            });
            return photo;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CameraService] PickPhotoAsync 错误: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 将文件结果保存到本地 AppDataDirectory 并返回文件路径
    /// </summary>
    /// <param name="fileResult">要保存的文件</param>
    /// <returns>保存后的本地路径，失败返回 null</returns>
    public async Task<string?> SaveToFileAsync(FileResult? fileResult)
    {
        if (fileResult == null)
            return null;

        try
        {
            // 确保保存目录存在
            var imagesDir = Path.Combine(FileSystem.AppDataDirectory, "images");
            if (!Directory.Exists(imagesDir))
            {
                Directory.CreateDirectory(imagesDir);
            }

            var fileName = $"FV_{DateTime.Now:yyyyMMdd_HHmmss}{Path.GetExtension(fileResult.FileName)}";
            var filePath = Path.Combine(imagesDir, fileName);

            using var stream = await fileResult.OpenReadAsync();
            using var fileStream = File.OpenWrite(filePath);
            await stream.CopyToAsync(fileStream);

            return filePath;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// 拍照并保存到本地，返回文件路径
    /// </summary>
    public async Task<string?> CaptureAndSaveAsync()
    {
        var result = await CapturePhotoAsync();
        return await SaveToFileAsync(result);
    }

    /// <summary>
    /// 从相册选取并保存到本地，返回文件路径
    /// </summary>
    public async Task<string?> PickAndSaveAsync()
    {
        var result = await PickPhotoAsync();
        return await SaveToFileAsync(result);
    }

    /// <summary>
    /// 设置闪光灯开关
    /// </summary>
    public async Task SetFlashAsync(bool on)
    {
#if ANDROID
        try
        {
            _isFlashOn = on;
            await SetFlashlightAsync(on);
        }
        catch
        {
            _isFlashOn = false;
        }
#endif
        await Task.CompletedTask;
    }

    /// <summary>
    /// 切换闪光灯开关（仅 Android）
    /// </summary>
    public async Task ToggleFlashAsync()
    {
#if ANDROID
        try
        {
            _isFlashOn = !_isFlashOn;
            await SetFlashlightAsync(_isFlashOn);
        }
        catch (Exception)
        {
            // 不支持时静默失败
            _isFlashOn = false;
        }
#endif
        await Task.CompletedTask;
    }

#if ANDROID
    /// <summary>
    /// Android 平台闪光灯控制
    /// </summary>
#pragma warning disable CA1416
    private async Task SetFlashlightAsync(bool on)
    {
        try
        {
            var context = Platform.AppContext;
            if (context == null) return;

            if (on)
            {
                // 尝试使用 CameraManager（API 23+）
                var cameraManager = (Android.Hardware.Camera2.CameraManager?)context.GetSystemService(Android.Content.Context.CameraService);
                if (cameraManager != null)
                {
                    var cameraIds = cameraManager.GetCameraIdList();
                    foreach (var id in cameraIds)
                    {
                        var characteristics = cameraManager.GetCameraCharacteristics(id);
                        var flashAvailable = characteristics.Get(Android.Hardware.Camera2.CameraCharacteristics.FlashInfoAvailable);
                        if (flashAvailable != null && (bool)flashAvailable)
                        {
                            cameraManager.SetTorchMode(id, true);
                            break;
                        }
                    }
                }
                else
                {
                    // 降级使用旧 API
                    var camera = Android.Hardware.Camera.Open();
                    if (camera != null)
                    {
                        var parameters = camera.GetParameters();
                        parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeTorch;
                        camera.SetParameters(parameters);
                        camera.StartPreview();
                    }
                }
            }
            else
            {
                // 关闭闪光灯
                var cameraManager = (Android.Hardware.Camera2.CameraManager?)context.GetSystemService(Android.Content.Context.CameraService);
                if (cameraManager != null)
                {
                    var cameraIds = cameraManager.GetCameraIdList();
                    foreach (var id in cameraIds)
                    {
                        cameraManager.SetTorchMode(id, false);
                    }
                }
            }
        }
        catch (Exception)
        {
            // 不支持时静默失败
        }

        await Task.CompletedTask;
    }
#pragma warning restore CA1416
#endif
}
