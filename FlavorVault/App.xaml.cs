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

        // Seed sample data on first launch
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

        // Restore user's saved font size setting on startup
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

        // Remove old theme dictionaries
        var oldThemes = dicts
            .Where(d => d.Source != null &&
                (d.Source.OriginalString.Contains("LightTheme") ||
                 d.Source.OriginalString.Contains("DarkTheme")))
            .ToList();
        foreach (var d in oldThemes)
            dicts.Remove(d);

        // Directly instantiate theme class (Source can only be set from XAML)
        ResourceDictionary themeDict = RequestedTheme == AppTheme.Dark
            ? new Resources.Themes.DarkTheme()
            : new Resources.Themes.LightTheme();

        // ICollection does not support Insert/indexer, save and rebuild
        var existing = dicts.ToList();
        dicts.Clear();
        // Theme dictionary first, then Styles etc.
        dicts.Add(themeDict);
        foreach (var d in existing)
            dicts.Add(d);
    }
}
