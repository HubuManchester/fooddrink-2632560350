using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlavorVault.Models;

namespace FlavorVault.ViewModels;

/// <summary>
/// 地点条目 ViewModel，用于 NearbyPlacesPage 列表项绑定
/// </summary>
public partial class PlaceItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _typeInitial = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _address = string.Empty;

    [ObservableProperty]
    private string _feature = string.Empty;

    [ObservableProperty]
    private string _distanceText = string.Empty;

    [ObservableProperty]
    private string _coordinatesText = string.Empty;

    [ObservableProperty]
    private Location? _location;

    /// <summary>
    /// 从 NearbyPlace 模型创建 ViewModel
    /// </summary>
    public static PlaceItemViewModel FromModel(NearbyPlace place)
    {
        return new PlaceItemViewModel
        {
            TypeInitial = GetTypeInitial(place.Type),
            Name = place.Name,
            Address = place.Address,
            Feature = place.Feature,
            DistanceText = place.DistanceText,
            CoordinatesText = place.CoordinatesText,
            Location = place.Location
        };
    }

    /// <summary>
    /// 根据地点类型返回首字
    /// </summary>
    private static string GetTypeInitial(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return "地";
        return type[..1];
    }

    [RelayCommand]
    private async Task OpenMap()
    {
        try
        {
            if (Location is not null)
            {
                var location = new Location(Location.Latitude, Location.Longitude);
                var options = new MapLaunchOptions { Name = Name };
                await Map.Default.OpenAsync(location, options);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PlaceItemViewModel] OpenMap 错误: {ex.Message}");
            try
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "提示", "无法打开地图应用", "确定");
            }
            catch { }
        }
    }
}
