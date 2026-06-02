using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// 稀有度 → 颜色（日常=#9E9E9E / 推荐=#42A5F5 / 限定=#AB47BC / 珍藏=#FFB300）
/// </summary>
public class RarityToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var rarity = value?.ToString() ?? string.Empty;

        return rarity switch
        {
            "日常" => Color.FromArgb("#9E9E9E"),
            "推荐" => Color.FromArgb("#42A5F5"),
            "限定" => Color.FromArgb("#AB47BC"),
            "珍藏" => Color.FromArgb("#FFB300"),
            _ => Color.FromArgb("#9E9E9E")
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "日常";
    }
}
