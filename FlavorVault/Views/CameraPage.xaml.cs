using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class CameraPage : ContentPage
{
    public CameraPage()
    {
        InitializeComponent();
        BindingContext = App.Current.Handler.MauiContext.Services.GetService<CameraViewModel>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}
