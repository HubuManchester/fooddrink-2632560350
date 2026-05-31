using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// 优先级 → 颜色（尽快=#F44336 / 有空就去=#FFC107 / 随缘=#4CAF50）
/// </summary>
public class PriorityToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var priority = value?.ToString() ?? string.Empty;

        return priority switch
        {
            "尽快" => Color.FromArgb("#F44336"),
            "有空就去" => Color.FromArgb("#FFC107"),
            "随缘" => Color.FromArgb("#4CAF50"),
            _ => Color.FromArgb("#FFC107")
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "有空就去";
    }
}
