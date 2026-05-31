using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// 色标名 → Color（红=#F44336 / 橙=#FF9800 / 黄=#FFEB3B / 绿=#4CAF50 / 蓝=#2196F3 / 紫=#9C27B0）
/// </summary>
public class ColorTagToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var tag = value?.ToString() ?? string.Empty;

        return tag switch
        {
            "红" => Color.FromArgb("#F44336"),
            "橙" => Color.FromArgb("#FF9800"),
            "黄" => Color.FromArgb("#FFEB3B"),
            "绿" => Color.FromArgb("#4CAF50"),
            "蓝" => Color.FromArgb("#2196F3"),
            "紫" => Color.FromArgb("#9C27B0"),
            _ => Color.FromArgb("#FF9800") // 默认橙色
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "橙";
    }
}
