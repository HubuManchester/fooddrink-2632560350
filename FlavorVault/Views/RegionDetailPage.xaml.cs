using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class RegionDetailPage : ContentPage
{
    public RegionDetailPage()
    {
        InitializeComponent();
        BindingContext = App.Current.Handler.MauiContext.Services.GetService<RegionDetailViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is RegionDetailViewModel vm)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
