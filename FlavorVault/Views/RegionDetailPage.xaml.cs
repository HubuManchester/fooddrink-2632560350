using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class RegionDetailPage : ContentPage, IQueryAttributable
{
    private string? _pendingRegion;

    public RegionDetailPage()
    {
        InitializeComponent();
        BindingContext = App.Current.Handler.MauiContext.Services.GetService<RegionDetailViewModel>();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("region", out var value))
        {
            var raw = value?.ToString() ?? string.Empty;
            _pendingRegion = Uri.UnescapeDataString(raw);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is RegionDetailViewModel vm && !string.IsNullOrWhiteSpace(_pendingRegion))
        {
            var region = _pendingRegion;
            _pendingRegion = null;
            await vm.LoadCommand.ExecuteAsync(region);
        }
    }
}
