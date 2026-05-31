using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// 收藏状态 → FA 图标 Unicode（已收藏=心形 / 想尝试=星形 / 已尝试=勾）
/// </summary>
public class CollectStatusToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var status = value?.ToString() ?? string.Empty;

        return status switch
        {
            "已收藏" => "",   // fa-heart (心形)
            "想尝试" => "",   // fa-star (星形)
            "已尝试" => "",   // fa-check (勾)
            _ => ""
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "已收藏";
    }
}
