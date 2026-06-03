using FlavorVault.Services;

namespace FlavorVault;

public partial class AppShell : Shell
{
    private bool _firstRunChecked = false;

    public AppShell()
    {
        InitializeComponent();

        // 注册层级路由
        Routing.RegisterRoute(nameof(Views.EntryDetailPage), typeof(Views.EntryDetailPage));
        Routing.RegisterRoute(nameof(Views.EntryEditPage), typeof(Views.EntryEditPage));
        Routing.RegisterRoute(nameof(Views.CameraPage), typeof(Views.CameraPage));
        Routing.RegisterRoute(nameof(Views.CollectionDetailPage), typeof(Views.CollectionDetailPage));
        Routing.RegisterRoute(nameof(Views.NearbyPlacesPage), typeof(Views.NearbyPlacesPage));
        Routing.RegisterRoute(nameof(Views.RegionDetailPage), typeof(Views.RegionDetailPage));
        Routing.RegisterRoute(nameof(Views.OnboardingPage), typeof(Views.OnboardingPage));
        Routing.RegisterRoute(nameof(Views.AboutPage), typeof(Views.AboutPage));

        // 初始化种子数据
        InitializeAsync();

        // 导航事件
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
        catch
        {
            // 种子数据初始化失败不阻塞启动
        }
    }

    private async void OnNavigated(object? sender, ShellNavigatedEventArgs e)
    {
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
        catch
        {
            // 首次运行检查失败不阻塞导航
        }
    }
}
