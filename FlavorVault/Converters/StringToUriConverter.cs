using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// string 路径 → ImageSource，支持 http URL 和本地文件路径
/// </summary>
public class StringToUriConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string path && !string.IsNullOrWhiteSpace(path))
        {
            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return ImageSource.FromUri(new Uri(path));
            }
            return ImageSource.FromFile(path);
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
