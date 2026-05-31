using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// int → FontSize
/// </summary>
public class IntToFontSizeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int size)
        {
            return (double)size;
        }
        return 14.0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return (int)d;
        }
        return 14;
    }
}
