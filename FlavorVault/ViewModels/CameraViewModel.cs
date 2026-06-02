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
        Title = "Photo Capture";
    }

    [RelayCommand]
    private async Task TakePhoto()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await Shell.Current.DisplayAlert("Notice", "Photo capture is not supported on this device, please select from gallery", "OK");
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
                await Shell.Current.DisplayAlert("Notice", "Photo capture cancelled or device not supported", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CameraViewModel] TakePhoto error: {ex.Message}\n{ex.StackTrace}");
            await Shell.Current.DisplayAlert("Photo Failed", ex.Message, "OK");
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
            System.Diagnostics.Debug.WriteLine($"[CameraViewModel] PickPhoto error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[CameraViewModel] SaveToCatalog error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[CameraViewModel] ToggleFlash error: {ex.Message}");
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
