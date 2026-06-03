using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class CatalogPage : ContentPage
{
    public CatalogPage()
    {
        InitializeComponent();
        BindingContext = App.Current?.Handler?.MauiContext?.Services?.GetService<CatalogViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CatalogViewModel vm)
        {
            await vm.LoadCommand.ExecuteAsync(null);
        }
    }
}
