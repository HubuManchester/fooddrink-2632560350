using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// 附近美食地点 ViewModel，支持定位、地点列表和地图跳转
/// </summary>
public partial class NearbyPlacesViewModel : BaseViewModel
{
    private readonly GeolocationService _geolocationService;
    private readonly PlaceMarkRepository _placeMarkRepo;

    [ObservableProperty]
    private string _currentLocation = "正在定位...";

    [ObservableProperty]
    private ObservableCollection<NearbyPlace> _nearbyPlaces = new();

    [ObservableProperty]
    private int _placeCount;

    /// <summary>
    /// 坐标文本（XAML 绑定用，CurrentLocation 的别名）
    /// </summary>
    public string CoordinatesText => CurrentLocation;

    /// <summary>
    /// 位置状态文本（XAML 绑定用，CurrentLocation 的别名）
    /// </summary>
    public string LocationStatus => CurrentLocation;

    /// <summary>
    /// 地点数量文本
    /// </summary>
    public string PlaceCountText => $"共 {PlaceCount} 个美食地标";

    /// <summary>
    /// 地点列表（XAML 绑定用，NearbyPlaces 的别名）
    /// </summary>
    public ObservableCollection<NearbyPlace> Places => NearbyPlaces;

    public NearbyPlacesViewModel(
        GeolocationService geolocationService,
        PlaceMarkRepository placeMarkRepo)
    {
        _geolocationService = geolocationService;
        _placeMarkRepo = placeMarkRepo;
        Title = "附近美食地标";
    }

    [RelayCommand]
    private async Task Load()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            // 获取当前位置
            try
            {
                var location = await _geolocationService.GetCurrentLocationAsync();
                if (location is not null)
                {
                    CurrentLocation = $"纬度 {location.Latitude:F6} 经度 {location.Longitude:F6}";
                }
                else
                {
                    CurrentLocation = "无法获取位置";
                }
                OnPropertyChanged(nameof(CoordinatesText));
                OnPropertyChanged(nameof(LocationStatus));
            }
            catch (Exception)
            {
                CurrentLocation = "定位未授权";
            }

            // 获取附近地点
            var places = await _geolocationService.GetNearbyPlacesAsync();
            NearbyPlaces.Clear();
            foreach (var place in places)
                NearbyPlaces.Add(place);

            PlaceCount = NearbyPlaces.Count;
            OnPropertyChanged(nameof(PlaceCountText));
            OnPropertyChanged(nameof(Places));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NearbyPlacesViewModel] Load 错误: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task OpenMap(NearbyPlace? place)
    {
        if (place is null) return;
        try
        {
            if (place.Location is not null)
            {
                var location = new Location(place.Location.Latitude, place.Location.Longitude);
                var options = new MapLaunchOptions { Name = place.Name };
                await Map.Default.OpenAsync(location, options);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NearbyPlacesViewModel] OpenMap 错误: {ex.Message}");
            try
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "提示", "无法打开地图应用", "确定");
            }
            catch { }
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await Load();
    }
}
