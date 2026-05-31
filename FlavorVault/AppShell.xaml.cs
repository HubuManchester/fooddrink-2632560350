using FlavorVault.Services;

namespace FlavorVault;

public partial class AppShell : Shell
{
    private bool _firstRunChecked = false;
    private bool _loginChecked = false;

    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.EntryDetailPage), typeof(Views.EntryDetailPage));
        Routing.RegisterRoute(nameof(Views.EntryEditPage), typeof(Views.EntryEditPage));
        Routing.RegisterRoute(nameof(Views.CameraPage), typeof(Views.CameraPage));
        Routing.RegisterRoute(nameof(Views.CollectionDetailPage), typeof(Views.CollectionDetailPage));
        Routing.RegisterRoute(nameof(Views.NearbyPlacesPage), typeof(Views.NearbyPlacesPage));
        Routing.RegisterRoute(nameof(Views.RegionDetailPage), typeof(Views.RegionDetailPage));
        Routing.RegisterRoute(nameof(Views.OnboardingPage), typeof(Views.OnboardingPage));
        Routing.RegisterRoute(nameof(Views.AboutPage), typeof(Views.AboutPage));
        Routing.RegisterRoute(nameof(Views.ShowcaseManagePage), typeof(Views.ShowcaseManagePage));
        Routing.RegisterRoute(nameof(Views.RegisterPage), typeof(Views.RegisterPage));
        Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));

        InitializeAsync();

        Navigated += OnNavigated;
    }

    private async void InitializeAsync()
    {
        try
        {
            var seedService = Handler?.MauiContext?.Services.GetService<SeedDataService>();
            if (seedService != null)
                await seedService.InitializeAsync();
        }
        catch { }
    }

    private async void OnNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        if (!_loginChecked)
        {
            _loginChecked = true;
            try
            {
                var authService = Handler?.MauiContext?.Services.GetService<AuthService>();
                if (authService != null)
                    await authService.TryRestoreSessionAsync();
            }
            catch { }
        }

        if (_firstRunChecked) return;
        _firstRunChecked = true;

        try
        {
            var firstRunService = Handler?.MauiContext?.Services.GetService<FirstRunService>();
            if (firstRunService != null && await firstRunService.IsFirstRunAsync())
            {
                await Current.GoToAsync($"//{nameof(Views.OnboardingPage)}");
            }
        }
        catch { }
    }
}
