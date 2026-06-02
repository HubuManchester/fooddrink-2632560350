using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// 星级 → 星星文本（传入 rating 和 index 参数，返回 ★ 或 ☆）
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
