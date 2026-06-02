using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// double distance to "X.X km" format string
/// </summary>
public class DistanceToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double distance)
        {
            return $"{distance:F1} km";
        }
        return "-- km";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return 0.0;
    }
}
