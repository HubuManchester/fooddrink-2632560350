using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// 字符串相等判断（parameter 为目标字符串）
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
