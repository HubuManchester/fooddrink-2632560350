using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// int 取反（0 → true，非0 → false）
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
