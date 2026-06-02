using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// Inverts int (0 to true, non-zero to false)
/// </summary>
public class InverseIntConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int i)
        {
            return i == 0;
        }
        return true;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && !b)
        {
            return 1;
        }
        return 0;
    }
}
