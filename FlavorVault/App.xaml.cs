using FlavorVault.Services;

namespace FlavorVault;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        ApplyTheme();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());

        // 首次启动时填充示例数据
        window.Created += async (_, _) =>
        {
            try
            {
                var seedService = IPlatformApplication.Current?.Services.GetService<Services.DataSeedService>();
                if (seedService is not null)
                    await seedService.SeedIfNeededAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] DataSeed error: {ex.Message}");
            }
        };

        return window;
    }

    protected override async void OnStart()
    {
        base.OnStart();

        // 启动时恢复用户保存的字号设置
        try
        {
            var themeService = IPlatformApplication.Current?.Services?.GetService<ThemeService>();
            if (themeService is not null)
            {
                var fontSize = await themeService.GetFontSizeAsync();
                themeService.ApplyFontSize(fontSize);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[App] OnStart font size error: {ex.Message}");
        }
    }

    private void ApplyTheme()
    {
        var dicts = Resources.MergedDictionaries;
        if (dicts == null) return;

        // 移除旧主题字典
        var oldThemes = dicts
            .Where(d => d.Source != null &&
                (d.Source.OriginalString.Contains("LightTheme") ||
                 d.Source.OriginalString.Contains("DarkTheme")))
            .ToList();
        foreach (var d in oldThemes)
            dicts.Remove(d);

        // 直接实例化主题类（Source 只能从 XAML 设置）
        ResourceDictionary themeDict = RequestedTheme == AppTheme.Dark
            ? new Resources.Themes.DarkTheme()
            : new Resources.Themes.LightTheme();

        // ICollection 不支持 Insert/索引器，先保存再重建
        var existing = dicts.ToList();
        dicts.Clear();
        // 主题字典在前，Styles 等在后
        dicts.Add(themeDict);
        foreach (var d in existing)
            dicts.Add(d);
    }
}
