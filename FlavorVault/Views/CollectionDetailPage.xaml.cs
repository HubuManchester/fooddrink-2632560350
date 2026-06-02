using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class CollectionDetailPage : ContentPage
{
    public CollectionDetailPage()
    {
        InitializeComponent();
        BindingContext = App.Current.Handler.MauiContext.Services.GetService<CollectionDetailViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CollectionDetailViewModel vm)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
