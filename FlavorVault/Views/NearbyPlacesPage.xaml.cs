using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class NearbyPlacesPage : ContentPage
{
    public NearbyPlacesPage()
    {
        InitializeComponent();
        BindingContext = App.Current.Handler.MauiContext.Services.GetService<NearbyPlacesViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is NearbyPlacesViewModel vm)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
