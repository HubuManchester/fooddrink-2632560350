using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
        BindingContext = App.Current?.Handler?.MauiContext?.Services?.GetService<AboutViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AboutViewModel vm)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
