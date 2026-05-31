using System.Globalization;

namespace FlavorVault.Converters;

/// <summary>
/// 地区 → FA 图标 Unicode 字符
/// </summary>
public class RegionToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var region = value?.ToString() ?? string.Empty;

        return region switch
        {
            "川渝" => "",   // fa-paw (麻辣风味)
            "粤港" => "",   // fa-cutlery (精致粤菜)
            "江南" => "",   // fa-leaf (江南水乡)
            "北方" => "",   // fa-bowl-food (北方豪放)
            "西北" => "",   // fa-mountain (西北大漠)
            "日本" => "",   // fa-fish (日式海鲜)
            "韩国" => "",   // fa-fire (韩式烤肉)
            "东南亚" => "", // fa-sun (热带风情)
            "欧美" => "",   // fa-globe (西式全球)
            _ => ""         // fa-cutlery (默认餐具)
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return string.Empty;
    }
}
