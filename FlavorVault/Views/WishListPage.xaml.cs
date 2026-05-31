using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class WishListPage : ContentPage
{
    public WishListPage()
    {
        InitializeComponent();
        BindingContext = App.Current.Handler.MauiContext.Services.GetService<WishListViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is WishListViewModel vm)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
