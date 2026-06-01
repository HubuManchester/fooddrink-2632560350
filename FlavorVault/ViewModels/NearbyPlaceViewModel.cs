using CommunityToolkit.Mvvm.ComponentModel;
using FlavorVault.Models;

namespace FlavorVault.ViewModels;

/// <summary>
/// 附近地点 ViewModel，用于 ExplorePage 横向卡片绑定
/// </summary>
public partial class NearbyPlaceViewModel : ObservableObject
{
    [ObservableProperty]
    private string _typeIcon = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private string _distanceText = string.Empty;

    /// <summary>
    /// 从 NearbyPlace 模型创建 ViewModel
    /// </summary>
    public static NearbyPlaceViewModel FromModel(NearbyPlace place)
    {
        return new NearbyPlaceViewModel
        {
            TypeIcon = GetTypeIcon(place.Type),
            Name = place.Name,
            Category = place.Category,
            DistanceText = place.DistanceText
        };
    }

    /// <summary>
    /// 根据地点类型返回 FontAwesome 图标 Unicode
    /// </summary>
    private static string GetTypeIcon(string? type)
    {
        return type switch
        {
            "餐厅" => "",
            "小吃" => "",
            "市场" => "",
            "甜品" => "",
            "饮品" => "",
            _ => ""
        };
    }
}
