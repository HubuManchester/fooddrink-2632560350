using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Media;

namespace FlavorVault.ViewModels;

public partial class CameraViewModel : BaseViewModel
{
    private readonly CameraService _cameraService;
    private readonly FoodEntryRepository _foodEntryRepo;

    [ObservableProperty]
    private string _capturedImagePath = string.Empty;

    [ObservableProperty]
    private bool _isFlashOn;

    public bool HasImage => !string.IsNullOrWhiteSpace(CapturedImagePath);

    public CameraViewModel(
        CameraService cameraService,
        FoodEntryRepository foodEntryRepo)
    {
        _cameraService = cameraService;
        _foodEntryRepo = foodEntryRepo;
        Title = "拍照识别";
    }

    [RelayCommand]
    private async Task TakePhoto()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await Shell.Current.DisplayAlert("提示", "当前设备不支持拍照，请使用相册选取", "确定");
                return;
            }

            var result = await _cameraService.CapturePhotoAsync();
            if (result is not null)
            {
                var path = await _cameraService.SaveToFileAsync(result);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    CapturedImagePath = path;
                    OnPropertyChanged(nameof(HasImage));
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("提示", "拍照取消或设备不支持", "确定");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CameraViewModel] TakePhoto 错误: {ex.Message}\n{ex.StackTrace}");
            await Shell.Current.DisplayAlert("拍照失败", ex.Message, "确定");
        }
    }

    [RelayCommand]
    private async Task PickPhoto()
    {
        try
        {
            var result = await _cameraService.PickPhotoAsync();
            if (result is not null)
            {
                var path = await _cameraService.SaveToFileAsync(result);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    CapturedImagePath = path;
                    OnPropertyChanged(nameof(HasImage));
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CameraViewModel] PickPhoto 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SaveToCatalog()
    {
        try
        {
            var imageParam = !string.IsNullOrWhiteSpace(CapturedImagePath) ? $"&imagePath={Uri.EscapeDataString(CapturedImagePath)}" : "";
            await Shell.Current.GoToAsync($"EntryEditPage?name={imageParam.TrimStart('&')}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CameraViewModel] SaveToCatalog 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ToggleFlash()
    {
        try
        {
            IsFlashOn = !IsFlashOn;
            await _cameraService.SetFlashAsync(IsFlashOn);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CameraViewModel] ToggleFlash 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Reset()
    {
        CapturedImagePath = string.Empty;
        IsFlashOn = false;
        OnPropertyChanged(nameof(HasImage));
    }
}
