using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// Rarity to color (Common=#9E9E9E / Recommended=#42A5F5 / Limited=#AB47BC / Premium=#FFB300)
/// </summary>
public class RarityToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var rarity = value?.ToString() ?? string.Empty;

        return rarity switch
        {
            "Common" => Color.FromArgb("#9E9E9E"),
            "Recommended" => Color.FromArgb("#42A5F5"),
            "Limited" => Color.FromArgb("#AB47BC"),
            "Premium" => Color.FromArgb("#FFB300"),
            _ => Color.FromArgb("#9E9E9E")
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "Common";
    }
}
