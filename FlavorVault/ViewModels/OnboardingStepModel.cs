using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.ViewModels;

/// <summary>
/// 引导步骤模型，用于 OnboardingPage CarouselView 绑定
/// </summary>
public partial class OnboardingStepModel : ObservableObject
{
    [ObservableProperty]
    private string _iconGlyph = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;
}
