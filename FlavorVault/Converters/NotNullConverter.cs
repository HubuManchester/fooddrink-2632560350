using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// 非 null → true
/// </summary>
public class NotNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null!;
    }
}
