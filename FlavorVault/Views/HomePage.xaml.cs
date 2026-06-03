using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        BindingContext = App.Current?.Handler?.MauiContext?.Services?.GetService<HomeViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is HomeViewModel vm)
        {
            await vm.LoadDataCommand.ExecuteAsync(null);
        }
    }
}
