using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class ShowcaseManagePage : ContentPage
{
    public ShowcaseManagePage()
    {
        InitializeComponent();
        BindingContext = App.Current?.Handler?.MauiContext?.Services?.GetService<ShowcaseManageViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ShowcaseManageViewModel vm)
        {
            await vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
