using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// Rating to star text (pass rating and index parameter, returns filled or empty star)
/// </summary>
public class RatingToStarConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        int rating = 0;
        if (value is int r)
        {
            rating = r;
        }

        int index = 0;
        if (parameter is string paramStr && int.TryParse(paramStr, out int idx))
        {
            index = idx;
        }
        else if (parameter is int paramInt)
        {
            index = paramInt;
        }

        return index < rating ? "" : ""; // fa-star / fa-star-o (solid/regular)
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return 0;
    }
}
