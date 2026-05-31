using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// 主味 → 颜色（咸=#2196F3 / 甜=#E91E63 / 酸=#FFC107 / 辣=#F44336 / 鲜=#4CAF50 / 复合=#9C27B0）
/// </summary>
public class TasteToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var taste = value?.ToString() ?? string.Empty;

        return taste switch
        {
            "咸" => Color.FromArgb("#2196F3"),
            "甜" => Color.FromArgb("#E91E63"),
            "酸" => Color.FromArgb("#FFC107"),
            "辣" => Color.FromArgb("#F44336"),
            "鲜" => Color.FromArgb("#4CAF50"),
            "复合" => Color.FromArgb("#9C27B0"),
            _ => Color.FromArgb("#9E9E9E")
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return string.Empty;
    }
}
