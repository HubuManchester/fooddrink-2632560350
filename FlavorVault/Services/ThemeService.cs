namespace FlavorVault.Services;

/// <summary>
/// App theme mode enum
/// </summary>
public enum ThemeMode
{
    Light,
    Dark,
    System
}

/// <summary>
/// Font size option enum
/// </summary>
public enum FontSizeOption
{
    Small,
    Medium,
    Large,
    ExtraLarge
}

/// <summary>
/// Theme and font size service
/// Persists user settings via UserProfileRepository
/// </summary>
public class ThemeService
{
    private readonly UserProfileRepository _userProfileRepository;

    /// <summary>
    /// Font size mapping: enum -> default size (unit: pixels)
    /// Small=12, Medium=14, Large=16, ExtraLarge=18
    /// </summary>
    private static readonly Dictionary<FontSizeOption, double> FontSizeMap = new()
    {
        { FontSizeOption.Small, 12 },
        { FontSizeOption.Medium, 14 },
        { FontSizeOption.Large, 16 },
        { FontSizeOption.ExtraLarge, 18 }
    };

    /// <summary>
    /// Base font size table (for DynamicResource FontSizeN), key is the resource Key
    /// </summary>
    private static readonly Dictionary<string, double> BaseFontSizes = new()
    {
        { "FontSize7", 7 }, { "FontSize9", 9 }, { "FontSize10", 10 },
        { "FontSize11", 11 }, { "FontSize12", 12 }, { "FontSize13", 13 },
        { "FontSize14", 14 }, { "FontSize15", 15 }, { "FontSize16", 16 },
        { "FontSize18", 18 }, { "FontSize20", 20 }, { "FontSize22", 22 },
        { "FontSize24", 24 }, { "FontSize28", 28 }, { "FontSize30", 30 },
        { "FontSize32", 32 }, { "FontSize36", 36 }, { "FontSize40", 40 },
        { "FontSize44", 44 }, { "FontSize48", 48 }, { "FontSize60", 60 }
    };

    /// <summary>
    /// Font size scale factor
    /// </summary>
    private static readonly Dictionary<FontSizeOption, double> FontScaleMap = new()
    {
        { FontSizeOption.Small, 0.85 },
        { FontSizeOption.Medium, 1.0 },
        { FontSizeOption.Large, 1.15 },
        { FontSizeOption.ExtraLarge, 1.3 }
    };

    public ThemeService(UserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    /// <summary>
    /// Get the current theme setting
    /// </summary>
    public async Task<ThemeMode> GetCurrentThemeAsync()
    {
        try
        {
            var themeStr = await _userProfileRepository.GetThemeAsync();
            return ParseThemeMode(themeStr);
        }
        catch (Exception)
        {
            return ThemeMode.Light;
        }
    }

    /// <summary>
    /// Set the theme and apply immediately
    /// </summary>
    /// <param name="theme">Target theme</param>
    public async Task SetThemeAsync(ThemeMode theme)
    {
        try
        {
            await _userProfileRepository.SetThemeAsync(theme.ToString());
            ApplyTheme(theme);
        }
        catch (Exception)
        {
            // Silent handling
        }
    }

    /// <summary>
    /// Get the font size setting
    /// </summary>
    public async Task<FontSizeOption> GetFontSizeAsync()
    {
        try
        {
            var sizeStr = await _userProfileRepository.GetFontSizeAsync();
            return ParseFontSize(sizeStr);
        }
        catch (Exception)
        {
            return FontSizeOption.Medium;
        }
    }

    /// <summary>
    /// Set the font size and apply immediately to the UI
    /// </summary>
    /// <param name="size">Font size option</param>
    public async Task SetFontSizeAsync(FontSizeOption size)
    {
        try
        {
            await _userProfileRepository.SetFontSizeAsync(size.ToString());
            ApplyFontSize(size);
        }
        catch (Exception)
        {
            // Silent handling
        }
    }

    /// <summary>
    /// Apply font size scaling to Application.Resources DynamicResource font size keys
    /// </summary>
    /// <param name="size">Font size option</param>
    public void ApplyFontSize(FontSizeOption size)
    {
        if (Application.Current?.Resources == null) return;

        var scale = FontScaleMap.GetValueOrDefault(size, 1.0);

        foreach (var kvp in BaseFontSizes)
        {
            Application.Current.Resources[kvp.Key] = Math.Round(kvp.Value * scale, 1);
        }
    }

    /// <summary>
    /// Apply the theme setting to Application.Current.UserThemeMode and swap resource dictionaries
    /// </summary>
    /// <param name="theme">The theme to apply</param>
    public void ApplyTheme(ThemeMode theme)
    {
        try
        {
            if (Application.Current == null) return;
            Application.Current.UserAppTheme = theme switch
            {
                ThemeMode.Light => Microsoft.Maui.ApplicationModel.AppTheme.Light,
                ThemeMode.Dark => Microsoft.Maui.ApplicationModel.AppTheme.Dark,
                _ => Microsoft.Maui.ApplicationModel.AppTheme.Unspecified,
            };

            SwapThemeDictionary(theme);
        }
        catch (Exception)
        {
            // Silent handling
        }
    }

    /// <summary>
    /// Swap app resource dictionaries: remove the old theme dictionary, insert the target theme dictionary.
    /// Pages using DynamicResource color keys will automatically update after switching.
    /// </summary>
    private void SwapThemeDictionary(ThemeMode theme)
    {
        var app = Application.Current;
        if (app?.Resources?.MergedDictionaries == null) return;

        var dicts = app.Resources.MergedDictionaries;

        // Determine the target theme
        var actualTheme = theme == ThemeMode.System
            ? (app.RequestedTheme == Microsoft.Maui.ApplicationModel.AppTheme.Dark
                ? ThemeMode.Dark
                : ThemeMode.Light)
            : theme;

        // Remove existing theme dictionaries (by type matching)
        var toRemove = dicts.Where(d =>
            d is Resources.Themes.LightTheme or Resources.Themes.DarkTheme).ToList();
        foreach (var d in toRemove)
            dicts.Remove(d);

        if (toRemove.Count == 0)
        {
            var sourceRemove = dicts.Where(d => d.Source != null &&
                (d.Source.OriginalString.Contains("LightTheme") ||
                 d.Source.OriginalString.Contains("DarkTheme"))).ToList();
            foreach (var d in sourceRemove)
                dicts.Remove(d);
        }

        ResourceDictionary newDict = actualTheme == ThemeMode.Dark
            ? new Resources.Themes.DarkTheme()
            : new Resources.Themes.LightTheme();

        try
        {
            dicts.Add(newDict);
        }
        catch (NullReferenceException)
        {
        }
    }

    /// <summary>
    /// Get the actual size value for a font size option
    /// </summary>
    /// <param name="size">Font size option</param>
    /// <returns>Font size in pixels</returns>
    public static double GetFontSizeValue(FontSizeOption size)
    {
        return FontSizeMap.GetValueOrDefault(size, 14);
    }

    /// <summary>
    /// Get the font size scale factor (based on Medium=14 as baseline)
    /// </summary>
    /// <param name="size">Font size option</param>
    /// <returns>Scale factor</returns>
    public static double GetFontScale(FontSizeOption size)
    {
        var value = GetFontSizeValue(size);
        return value / 14.0; // Using Medium=14 as baseline
    }

    private static ThemeMode ParseThemeMode(string? themeStr)
    {
        if (string.IsNullOrWhiteSpace(themeStr))
            return ThemeMode.Light;

        return Enum.TryParse<ThemeMode>(themeStr, ignoreCase: true, out var result)
            ? result
            : ThemeMode.Light;
    }

    private static FontSizeOption ParseFontSize(string? sizeStr)
    {
        if (string.IsNullOrWhiteSpace(sizeStr))
            return FontSizeOption.Medium;

        return Enum.TryParse<FontSizeOption>(sizeStr, ignoreCase: true, out var result)
            ? result
            : FontSizeOption.Medium;
    }
}
