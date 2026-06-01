using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class OnboardingPage : ContentPage
{
    public OnboardingPage()
    {
        InitializeComponent();
        BindingContext = App.Current?.Handler?.MauiContext?.Services?.GetService<OnboardingViewModel>();
    }
}
