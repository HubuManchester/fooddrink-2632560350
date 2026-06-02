using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// 附近美食地点（非数据库模型）
/// </summary>
public partial class NearbyPlace : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _address = string.Empty;

    /// <summary>
    /// 距离（单位：km）
    /// </summary>
    [ObservableProperty]
    private double _distance;

    /// <summary>
    /// 地点类型缩写
    /// </summary>
    [ObservableProperty]
    private string _type = string.Empty;

    /// <summary>
    /// 距离文本，格式 "X.X km"
    /// </summary>
    public string DistanceText => $"{Distance:F1} km";

    /// <summary>
    /// 坐标文本，格式 "纬度xx.xxxxxx 经度xx.xxxxxx"
    /// </summary>
    public string CoordinatesText =>
        Location != null
            ? $"纬度{Location.Latitude:F6} 经度{Location.Longitude:F6}"
            : string.Empty;

    [ObservableProperty]
    private Location? _location;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private string _region = string.Empty;

    [ObservableProperty]
    private string _feature = string.Empty;

    [ObservableProperty]
    private int _starRating = 3;
}
