using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// Primary taste to color (Salty=#2196F3 / Sweet=#E91E63 / Sour=#FFC107 / Spicy=#F44336 / Umami=#4CAF50 / Complex=#9C27B0)
/// </summary>
public class TasteToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var taste = value?.ToString() ?? string.Empty;

        return taste switch
        {
            "Salty" => Color.FromArgb("#2196F3"),
            "Sweet" => Color.FromArgb("#E91E63"),
            "Sour" => Color.FromArgb("#FFC107"),
            "Spicy" => Color.FromArgb("#F44336"),
            "Umami" => Color.FromArgb("#4CAF50"),
            "Complex" => Color.FromArgb("#9C27B0"),
            _ => Color.FromArgb("#9E9E9E")
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return string.Empty;
    }
}
