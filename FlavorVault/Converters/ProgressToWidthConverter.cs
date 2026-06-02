using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// Progress percentage to width (parameter is total width)
/// </summary>
public class ProgressToWidthConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        double percent = 0;
        if (value is double d)
        {
            percent = d;
        }
        else if (value is int i)
        {
            percent = i;
        }
        else if (value is float f)
        {
            percent = f;
        }

        double totalWidth = 200;
        if (parameter is string paramStr && double.TryParse(paramStr, out double pw))
        {
            totalWidth = pw;
        }
        else if (parameter is double pd)
        {
            totalWidth = pd;
        }
        else if (parameter is int pi)
        {
            totalWidth = pi;
        }

        // percent is a value from 0~100
        var width = (percent / 100.0) * totalWidth;
        return Math.Max(0, Math.Min(width, totalWidth));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return 0.0;
    }
}
