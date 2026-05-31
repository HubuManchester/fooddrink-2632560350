using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// double 距离 → "X.X km" 格式字符串
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
