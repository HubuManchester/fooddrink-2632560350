using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.ViewModels;

/// <summary>
/// Onboarding step model, used for OnboardingPage CarouselView binding
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
