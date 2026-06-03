using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// 拍照识别 ViewModel，支持拍照、相册选取、YOLO 推理和保存到图鉴
/// </summary>
public partial class CameraViewModel : BaseViewModel
{
    private readonly CameraService _cameraService;
    private readonly YoloInferenceService _yoloService;
    private readonly FoodEntryRepository _foodEntryRepo;

    [ObservableProperty]
    private string _capturedImagePath = string.Empty;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private string _recognizedLabel = string.Empty;

    [ObservableProperty]
    private float _confidence;

    [ObservableProperty]
    private string _mappedName = string.Empty;

    [ObservableProperty]
    private bool _isFlashOn;

    [ObservableProperty]
    private bool _hasResult;

    /// <summary>
    /// 是否已有拍摄图片（XAML 绑定用）
    /// </summary>
    public bool HasImage => !string.IsNullOrWhiteSpace(CapturedImagePath);

    /// <summary>
    /// 是否正在识别中（XAML 绑定用，IsProcessing 的别名）
    /// </summary>
    public bool IsRecognizing => IsProcessing;

    /// <summary>
    /// 是否有识别结果（XAML 绑定用，HasResult 的别名）
    /// </summary>
    public bool HasRecognition => HasResult;

    /// <summary>
    /// 识别后的食物名称（XAML 绑定用，优先显示 MappedName）
    /// </summary>
    public string RecognizedName => !string.IsNullOrWhiteSpace(MappedName) ? MappedName : RecognizedLabel;

    /// <summary>
    /// 置信度文本
    /// </summary>
    public string ConfidenceText => Confidence > 0 ? $"置信度 {Confidence:P0}" : string.Empty;

    /// <summary>
    /// 推荐地区
    /// </summary>
    [ObservableProperty]
    private string _suggestedRegion = string.Empty;

    /// <summary>
    /// 推荐稀有度
    /// </summary>
    [ObservableProperty]
    private string _suggestedRarity = string.Empty;

    public CameraViewModel(
        CameraService cameraService,
        YoloInferenceService yoloService,
        FoodEntryRepository foodEntryRepo)
    {
        _cameraService = cameraService;
        _yoloService = yoloService;
        _foodEntryRepo = foodEntryRepo;
        Title = "拍照识别";
    }

    [RelayCommand]
    private async Task TakePhoto()
    {
        try
        {
            var result = await _cameraService.CapturePhotoAsync();
            if (result is not null)
            {
                var path = await _cameraService.SaveToFileAsync(result);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    CapturedImagePath = path;
                    HasResult = false;
                    OnPropertyChanged(nameof(HasImage));
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CameraViewModel] TakePhoto 错误: {ex.Message}");
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
                    HasResult = false;
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
    private async Task Recognize()
    {
        if (string.IsNullOrWhiteSpace(CapturedImagePath)) return;
        if (IsProcessing) return;

        IsProcessing = true;
        OnPropertyChanged(nameof(IsRecognizing));
        try
        {
            var predictions = await _yoloService.PredictAsync(CapturedImagePath);

            if (predictions is not null && predictions.Count > 0)
            {
                var topPrediction = predictions.OrderByDescending(p => p.Confidence).First();
                RecognizedLabel = topPrediction.Label;
                Confidence = topPrediction.Confidence;
                MappedName = _yoloService.MapLabelToChinese(topPrediction.Label) ?? topPrediction.Label;
                HasResult = true;
                SuggestedRegion = "未知";
                SuggestedRarity = "日常";
                OnPropertyChanged(nameof(HasImage));
                OnPropertyChanged(nameof(IsRecognizing));
                OnPropertyChanged(nameof(HasRecognition));
                OnPropertyChanged(nameof(RecognizedName));
                OnPropertyChanged(nameof(ConfidenceText));
            }
            else
            {
                RecognizedLabel = string.Empty;
                MappedName = string.Empty;
                Confidence = 0;
                HasResult = false;

                await Application.Current!.MainPage!.DisplayAlert(
                    "识别结果", "未能识别出食物，请尝试重新拍照", "确定");
            }
        }
        catch (InvalidOperationException ex)
        {
            // 模型文件缺失
            await Application.Current!.MainPage!.DisplayAlert(
                "模型不可用", "YOLO 模型文件未找到，请将模型文件放置到正确目录后重试", "确定");
            System.Diagnostics.Debug.WriteLine($"[CameraViewModel] Recognize 模型缺失: {ex.Message}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CameraViewModel] Recognize 错误: {ex.Message}");
            await Application.Current!.MainPage!.DisplayAlert(
                "识别失败", "识别过程出现错误，请重试", "确定");
        }
        finally
        {
            IsProcessing = false;
            OnPropertyChanged(nameof(IsRecognizing));
        }
    }

    [RelayCommand]
    private async Task SaveToCatalog()
    {
        try
        {
            var name = !string.IsNullOrWhiteSpace(MappedName) ? MappedName : RecognizedLabel;
            await Shell.Current.GoToAsync($"EntryEditPage?name={Uri.EscapeDataString(name)}");
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
        RecognizedLabel = string.Empty;
        MappedName = string.Empty;
        Confidence = 0;
        HasResult = false;
        IsProcessing = false;
        SuggestedRegion = string.Empty;
        SuggestedRarity = string.Empty;
        OnPropertyChanged(nameof(HasImage));
        OnPropertyChanged(nameof(HasRecognition));
        OnPropertyChanged(nameof(IsRecognizing));
        OnPropertyChanged(nameof(RecognizedName));
        OnPropertyChanged(nameof(ConfidenceText));
    }
}
