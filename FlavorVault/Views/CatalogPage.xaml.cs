using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class CatalogPage : ContentPage
{
    public CatalogPage()
    {
        InitializeComponent();
        BindingContext = App.Current?.Handler?.MauiContext?.Services?.GetService<CatalogViewModel>();

#if WINDOWS
        PhotoGrid.ItemsLayout = new GridItemsLayout(3, ItemsLayoutOrientation.Vertical)
        {
            HorizontalItemSpacing = 4,
            VerticalItemSpacing = 4
        };
        CatalogList.ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical)
        {
            HorizontalItemSpacing = 8,
            VerticalItemSpacing = 8
        };
#endif
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
