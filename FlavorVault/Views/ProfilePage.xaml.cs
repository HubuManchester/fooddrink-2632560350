using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = App.Current.Handler.MauiContext.Services.GetService<ProfileViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfileViewModel vm)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
