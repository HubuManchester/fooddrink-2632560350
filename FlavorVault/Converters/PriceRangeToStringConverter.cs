using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// 价格区间 → 显示文字（"~10"→"10元以下" 等）
/// </summary>
public class PriceRangeToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var range = value?.ToString() ?? string.Empty;

        return range switch
        {
            "~10" => "10元以下",
            "10~30" => "10-30元",
            "30~60" => "30-60元",
            "60~100" => "60-100元",
            "100+" => "100元以上",
            _ => range
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "10~30";
    }
}
