namespace FlavorVault.Services;

/// <summary>
/// 应用主题模式枚举
/// </summary>
public enum ThemeMode
{
    Light,
    Dark,
    System
}

/// <summary>
/// 字体大小选项枚举
/// </summary>
public enum FontSizeOption
{
    Small,
    Medium,
    Large,
    ExtraLarge
}

/// <summary>
/// 主题与字体大小服务
/// 通过 UserProfileRepository 持久化用户设置
/// </summary>
public class ThemeService
{
    private readonly UserProfileRepository _userProfileRepository;

    /// <summary>
    /// 字体大小映射表：枚举 -> 默认尺寸（单位：像素）
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
    /// 基础字号表（用于 DynamicResource FontSizeN），键名为资源 Key
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
    /// 字号缩放倍率
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
    /// 获取当前主题设置
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
    /// 设置主题并立即应用
    /// </summary>
    /// <param name="theme">目标主题</param>
    public async Task SetThemeAsync(ThemeMode theme)
    {
        try
        {
            await _userProfileRepository.SetThemeAsync(theme.ToString());
            ApplyTheme(theme);
        }
        catch (Exception)
        {
            // 静默处理
        }
    }

    /// <summary>
    /// 获取字体大小设置
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
    /// 设置字体大小并立即应用到 UI
    /// </summary>
    /// <param name="size">字体大小选项</param>
    public async Task SetFontSizeAsync(FontSizeOption size)
    {
        try
        {
            await _userProfileRepository.SetFontSizeAsync(size.ToString());
            ApplyFontSize(size);
        }
        catch (Exception)
        {
            // 静默处理
        }
    }

    /// <summary>
    /// 将字号缩放应用到 Application.Resources 中的 DynamicResource 字号键
    /// </summary>
    /// <param name="size">字体大小选项</param>
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
    /// 将主题设置应用到 Application.Current.UserThemeMode 并切换资源字典
    /// </summary>
    /// <param name="theme">要应用的主题</param>
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
            // 静默处理
        }
    }

    /// <summary>
    /// 切换应用资源字典：移除旧主题字典，插入目标主题字典。
    /// 页面使用 DynamicResource 引用颜色键即可在切换后自动更新。
    /// </summary>
    private void SwapThemeDictionary(ThemeMode theme)
    {
        var app = Application.Current;
        if (app?.Resources?.MergedDictionaries == null) return;

        var dicts = app.Resources.MergedDictionaries;

        // 确定目标主题
        var actualTheme = theme == ThemeMode.System
            ? (app.RequestedTheme == Microsoft.Maui.ApplicationModel.AppTheme.Dark
                ? ThemeMode.Dark
                : ThemeMode.Light)
            : theme;

        // 移除现有的主题字典（通过类型匹配）
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
    /// 获取字体大小对应的实际尺寸值
    /// </summary>
    /// <param name="size">字体大小选项</param>
    /// <returns>字体尺寸（像素）</returns>
    public static double GetFontSizeValue(FontSizeOption size)
    {
        return FontSizeMap.GetValueOrDefault(size, 14);
    }

    /// <summary>
    /// 获取字体尺寸倍率（基于 Medium=14 为基准）
    /// </summary>
    /// <param name="size">字体大小选项</param>
    /// <returns>倍率</returns>
    public static double GetFontScale(FontSizeOption size)
    {
        var value = GetFontSizeValue(size);
        return value / 14.0; // 以 Medium=14 为基准
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
