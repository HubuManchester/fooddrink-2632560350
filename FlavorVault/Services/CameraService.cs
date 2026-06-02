using Microsoft.Maui.Media;
using Microsoft.Maui.ApplicationModel;

namespace FlavorVault.Services;

/// <summary>
/// Camera service using Microsoft.Maui.Media.MediaPicker
/// Includes photo capture, album picking, file saving, and Android flashlight control
/// </summary>
public class CameraService
{
#if ANDROID
    private bool _isFlashOn;
#endif

    /// <summary>
    /// Whether the flashlight is on (only effective on Android)
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
    /// Capture a photo and return the file result
    /// </summary>
    /// <returns>Photo result, or null on failure or cancellation</returns>
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
            System.Diagnostics.Debug.WriteLine($"[CameraService] CapturePhotoAsync error: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Pick a photo from the album
    /// </summary>
    /// <returns>Pick result, or null on cancellation</returns>
    public async Task<FileResult?> PickPhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Select Photo"
            });
            return photo;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CameraService] PickPhotoAsync error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Save the file result to the local AppDataDirectory and return the file path
    /// </summary>
    /// <param name="fileResult">The file to save</param>
    /// <returns>Local path after saving, or null on failure</returns>
    public async Task<string?> SaveToFileAsync(FileResult? fileResult)
    {
        if (fileResult == null)
            return null;

        try
        {
            // Ensure the save directory exists
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
    /// Capture a photo and save it locally, returning the file path
    /// </summary>
    public async Task<string?> CaptureAndSaveAsync()
    {
        var result = await CapturePhotoAsync();
        return await SaveToFileAsync(result);
    }

    /// <summary>
    /// Pick a photo from the album and save it locally, returning the file path
    /// </summary>
    public async Task<string?> PickAndSaveAsync()
    {
        var result = await PickPhotoAsync();
        return await SaveToFileAsync(result);
    }

    /// <summary>
    /// Set the flashlight on/off
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
    /// Toggle the flashlight on/off (Android only)
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
            // Silently fail when not supported
            _isFlashOn = false;
        }
#endif
        await Task.CompletedTask;
    }

#if ANDROID
    /// <summary>
    /// Android platform flashlight control
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
                // Try using CameraManager (API 23+)
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
                    // Fallback to legacy API
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
                // Turn off the flashlight
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
            // Silently fail when not supported
        }

        await Task.CompletedTask;
    }
#pragma warning restore CA1416
#endif
}
