using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// String equality check (parameter is the target string)
/// </summary>
public class StringEqualsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var str = value?.ToString() ?? string.Empty;
        var target = parameter?.ToString() ?? string.Empty;
        return string.Equals(str, target, StringComparison.OrdinalIgnoreCase);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return false;
    }
}
