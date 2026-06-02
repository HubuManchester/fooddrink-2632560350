using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// Price range to display text ("~10" to "Under ¥10" etc.)
/// </summary>
public class PriceRangeToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var range = value?.ToString() ?? string.Empty;

        return range switch
        {
            "~10" => "Under ¥10",
            "10~30" => "¥10-30",
            "30~60" => "¥30-60",
            "60~100" => "¥60-100",
            "100+" => "Over ¥100",
            _ => range
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "10~30";
    }
}
