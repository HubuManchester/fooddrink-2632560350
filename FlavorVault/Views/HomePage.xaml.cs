using FlavorVault.Services;
using FlavorVault.ViewModels;

namespace FlavorVault.Views;

public partial class HomePage : ContentPage
{
    private readonly SensorService _sensorService;

    public HomePage()
    {
        InitializeComponent();
        _sensorService = App.Current?.Handler?.MauiContext?.Services?.GetService<SensorService>() ?? new SensorService();
        BindingContext = App.Current?.Handler?.MauiContext?.Services?.GetService<HomeViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is HomeViewModel vm)
        {
            await vm.LoadDataCommand.ExecuteAsync(null);
            _sensorService.StartShakeDetection(() => MainThread.BeginInvokeOnMainThread(async () =>
            {
                await vm.ShakeDiscoverCommand.ExecuteAsync(null);
            }));
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _sensorService.StopShakeDetection();
    }
}
