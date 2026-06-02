using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class ExplorePage : ContentPage
{
    public ExplorePage()
    {
        InitializeComponent();
        BindingContext = App.Current.Handler.MauiContext.Services.GetService<ExploreViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ExploreViewModel vm)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
