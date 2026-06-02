using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// Region to FA icon Unicode character
/// </summary>
public class RegionToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var region = value?.ToString() ?? string.Empty;

        return region switch
        {
            "Sichuan" => "",       // fa-paw (spicy flavor)
            "Cantonese" => "",     // fa-cutlery (refined Cantonese cuisine)
            "Jiangnan" => "",      // fa-leaf (Jiangnan water town)
            "Northern" => "",      // fa-bowl-food (hearty Northern style)
            "Northwest" => "",     // fa-mountain (Northwest desert)
            "Japan" => "",         // fa-fish (Japanese seafood)
            "Korea" => "",         // fa-fire (Korean BBQ)
            "SoutheastAsia" => "", // fa-sun (tropical vibes)
            "Western" => "",       // fa-globe (Western global)
            _ => ""                // fa-cutlery (default utensils)
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return string.Empty;
    }
}
