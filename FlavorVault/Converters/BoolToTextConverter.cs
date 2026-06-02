using System.Globalization;

namespace FlavorVault.Converters;

public class BoolToTextConverter : IValueConverter
{
    public string TrueText { get; set; } = "\uf06e";
    public string FalseText { get; set; } = "\uf070";

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? TrueText : FalseText;
        return FalseText;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
