using System.Collections.ObjectModel;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// 引导页 ViewModel，管理3步引导流程
/// </summary>
public partial class OnboardingViewModel : BaseViewModel
{
    private readonly FirstRunService _firstRunService;

    [ObservableProperty]
    private int _currentStep;

    [ObservableProperty]
    private int _stepCount = 3;

    [ObservableProperty]
    private bool _isLastStep;

    /// <summary>
    /// 引导步骤列表
    /// </summary>
    public ObservableCollection<OnboardingStepModel> Steps { get; }

    /// <summary>
    /// 当前步骤索引（XAML 绑定用，CurrentStep 的别名）
    /// </summary>
    public int CurrentIndex => CurrentStep;

    /// <summary>
    /// 是否是第一步
    /// </summary>
    public bool IsFirstStep => CurrentStep == 0;

    /// <summary>
    /// 是否是第二步
    /// </summary>
    public bool IsSecondStep => CurrentStep == 1;

    /// <summary>
    /// 是否是第三步
    /// </summary>
    public bool IsThirdStep => CurrentStep == 2;

    /// <summary>
    /// 是否不是最后一步（XAML 绑定用，"跳过"按钮可见性）
    /// </summary>
    public bool IsNotLastStep => !IsLastStep;

    /// <summary>
    /// 下一步按钮文字
    /// </summary>
    public string NextButtonText => IsLastStep ? "开始使用" : "下一步";

    public OnboardingViewModel(FirstRunService firstRunService)
    {
        _firstRunService = firstRunService;
        Title = "欢迎";

        // 初始化引导步骤
        Steps = new ObservableCollection<OnboardingStepModel>
        {
            new() { IconGlyph = "", Title = "欢迎来到味藏", Description = "你的专属食物收藏图鉴，像编撰百科全书一样记录每一道美食。" },
            new() { IconGlyph = "", Title = "拍照识别", Description = "使用 AI 拍照识别美食，自动录入到你的图鉴中。" },
            new() { IconGlyph = "", Title = "探索发现", Description = "发现附近的美食地标，探索不同地区的特色风味。" }
        };
    }

    [RelayCommand]
    private async Task Next()
    {
        try
        {
            CurrentStep++;

            OnPropertyChanged(nameof(CurrentIndex));

            // 更新所有步骤相关计算属性
            OnPropertyChanged(nameof(IsFirstStep));
            OnPropertyChanged(nameof(IsSecondStep));
            OnPropertyChanged(nameof(IsThirdStep));
            OnPropertyChanged(nameof(IsNotLastStep));
            OnPropertyChanged(nameof(NextButtonText));

            if (CurrentStep >= StepCount - 1)
            {
                IsLastStep = true;
                OnPropertyChanged(nameof(IsNotLastStep));
                OnPropertyChanged(nameof(NextButtonText));
            }

            if (CurrentStep >= StepCount)
            {
                // 最后一步，标记完成并导航到首页
                await CompleteOnboarding();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[OnboardingViewModel] Next 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Skip()
    {
        try
        {
            await CompleteOnboarding();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[OnboardingViewModel] Skip 错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 完成引导，标记首次运行并导航到首页
    /// </summary>
    private async Task CompleteOnboarding()
    {
        try
        {
            await _firstRunService.MarkCompletedAsync();
            await Shell.Current.GoToAsync("//HomePage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[OnboardingViewModel] CompleteOnboarding 错误: {ex.Message}");
        }
    }
}
