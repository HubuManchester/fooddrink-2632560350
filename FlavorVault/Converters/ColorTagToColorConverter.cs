using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// Color tag name to Color (Red=#F44336 / Orange=#FF9800 / Yellow=#FFEB3B / Green=#4CAF50 / Blue=#2196F3 / Purple=#9C27B0)
/// </summary>
public class ColorTagToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var tag = value?.ToString() ?? string.Empty;

        return tag switch
        {
            "Red" => Color.FromArgb("#F44336"),
            "Orange" => Color.FromArgb("#FF9800"),
            "Yellow" => Color.FromArgb("#FFEB3B"),
            "Green" => Color.FromArgb("#4CAF50"),
            "Blue" => Color.FromArgb("#2196F3"),
            "Purple" => Color.FromArgb("#9C27B0"),
            _ => Color.FromArgb("#FF9800") // Default orange
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "Orange";
    }
}
