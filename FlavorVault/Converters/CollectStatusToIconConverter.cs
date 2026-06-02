using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// Collect status to FA icon Unicode (Collected=heart / WantToTry=star / Tried=check)
/// </summary>
public class CollectStatusToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var status = value?.ToString() ?? string.Empty;

        return status switch
        {
            "Collected" => "",   // fa-heart (heart)
            "WantToTry" => "",   // fa-star (star)
            "Tried" => "",   // fa-check (check)
            _ => ""
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "Collected";
    }
}
