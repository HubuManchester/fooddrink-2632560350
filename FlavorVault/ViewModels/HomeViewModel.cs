using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;

namespace FlavorVault.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly FoodEntryRepository _foodEntryRepo;
    private readonly CollectionRepository _collectionRepo;
    private readonly PlaceMarkRepository _placeMarkRepo;
    private readonly CatalogCalculator _calculator;
    private readonly GeolocationService _geolocationService;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private ObservableCollection<RegionProgress> _regionProgresses = new();

    [ObservableProperty]
    private ObservableCollection<RarityCount> _rarityStats = new();

    [ObservableProperty]
    private ObservableCollection<FoodEntry> _showcaseEntries = new();

    [ObservableProperty]
    private FoodEntry? _shakeResult;

    [ObservableProperty]
    private bool _hasShakeResult;

    /// <summary>
    /// 是否正在刷新（XAML 绑定用）
    /// </summary>
    [ObservableProperty]
    private bool _isRefreshing;

    /// <summary>
    /// 条目数量文本（XAML 绑定用）
    /// </summary>
    public string EntryCountText => $"已收录 {TotalCount} 道美食";

    public HomeViewModel(
        FoodEntryRepository foodEntryRepo,
        CollectionRepository collectionRepo,
        PlaceMarkRepository placeMarkRepo,
        CatalogCalculator calculator,
        GeolocationService geolocationService)
    {
        _foodEntryRepo = foodEntryRepo;
        _collectionRepo = collectionRepo;
        _placeMarkRepo = placeMarkRepo;
        _calculator = calculator;
        _geolocationService = geolocationService;
        Title = "我的图鉴";
    }

    [RelayCommand]
    private async Task LoadData()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var allEntries = await _foodEntryRepo.GetAllAsync();

            // 总数
            TotalCount = allEntries.Count;
            OnPropertyChanged(nameof(EntryCountText));

            // 地区进度
            var progresses = _calculator.CalculateRegionProgress(allEntries);
            RegionProgresses.Clear();
            foreach (var p in progresses)
                RegionProgresses.Add(p);

            // 稀有度统计
            var rarities = _calculator.CalculateRarityStats(allEntries);
            RarityStats.Clear();
            foreach (var r in rarities)
                RarityStats.Add(r);

            // 橱窗精选（IsShowcase=true，最多5个）
            var showcases = allEntries
                .Where(e => e.IsShowcase)
                .Take(5)
                .ToList();
            ShowcaseEntries.Clear();
            foreach (var entry in showcases)
                ShowcaseEntries.Add(entry);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] LoadData 错误: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadData();
    }

    [RelayCommand]
    private async Task GoToCamera()
    {
        try
        {
            await Shell.Current.GoToAsync("CameraPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToCamera 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToCatalog()
    {
        try
        {
            await Shell.Current.GoToAsync("//CatalogPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToCatalog 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToShowcaseManage()
    {
        try
        {
            await Shell.Current.GoToAsync("ShowcaseManagePage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToShowcaseManage 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToNewEntry()
    {
        try
        {
            await Shell.Current.GoToAsync("EntryEditPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToNewEntry 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToEntryDetail(FoodEntry? entry)
    {
        if (entry is null) return;
        try
        {
            await Shell.Current.GoToAsync($"EntryDetailPage?id={entry.Id}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToEntryDetail 错误: {ex.Message}");
        }
    }

    [ObservableProperty]
    private string _shakeStars = string.Empty;

    [ObservableProperty]
    private bool _hasLocationInfo;

    [ObservableProperty]
    private string _locationCity = string.Empty;

    [ObservableProperty]
    private string _locationCoordinates = string.Empty;

    [ObservableProperty]
    private Location? _currentLocation;

    [RelayCommand]
    private async Task GoToExplore()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var location = await _geolocationService.GetCurrentLocationAsync();
            if (location is not null)
            {
                CurrentLocation = location;
                LocationCity = GetCityFromCoordinates(location.Latitude, location.Longitude);
                LocationCoordinates = $"纬度 {location.Latitude:F6}  经度 {location.Longitude:F6}";
            }
            else
            {
                CurrentLocation = null;
                LocationCity = "无法定位";
                LocationCoordinates = "请检查位置权限";
            }
            HasLocationInfo = true;
        }
        catch (Exception)
        {
            LocationCity = "定位失败";
            LocationCoordinates = "请检查位置权限";
            HasLocationInfo = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task OpenMap()
    {
        try
        {
            var loc = CurrentLocation ?? new Location(39.9042, 116.4074);
            await Map.Default.OpenAsync(loc, new MapLaunchOptions { Name = LocationCity });
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("提示", "无法打开地图应用", "确定");
        }
    }

    [RelayCommand]
    private void CloseLocationInfo()
    {
        HasLocationInfo = false;
    }

    private static readonly (string Name, double Lat, double Lon)[] KnownCities = new[]
    {
        ("成都", 30.57, 104.07), ("重庆", 29.56, 106.55),
        ("广州", 23.13, 113.26), ("深圳", 22.55, 114.06), ("香港", 22.32, 114.17),
        ("上海", 31.23, 121.47), ("杭州", 30.27, 120.15), ("南京", 32.06, 118.80),
        ("北京", 39.90, 116.40), ("天津", 39.08, 117.20),
        ("西安", 34.26, 108.94), ("兰州", 36.06, 103.83),
        ("武汉", 30.59, 114.30), ("长沙", 28.23, 112.94),
        ("成都", 30.57, 104.07), ("昆明", 25.04, 102.68),
        ("哈尔滨", 45.75, 126.65), ("沈阳", 41.80, 123.43),
        ("大连", 38.91, 121.60), ("青岛", 36.07, 120.38),
        ("厦门", 24.48, 118.09), ("福州", 26.07, 119.30),
        ("郑州", 34.75, 113.65), ("济南", 36.65, 116.99),
        ("合肥", 31.82, 117.23), ("南昌", 28.68, 115.86),
        ("贵阳", 26.65, 106.63), ("南宁", 22.82, 108.37),
        ("海口", 20.04, 110.35), ("三亚", 18.25, 109.50),
        ("拉萨", 29.65, 91.10), ("乌鲁木齐", 43.83, 87.62),
        ("呼和浩特", 40.84, 111.75), ("石家庄", 38.04, 114.51),
        ("太原", 37.87, 112.55), ("长春", 43.88, 125.32),
        ("苏州", 31.30, 120.62), ("无锡", 31.49, 120.31),
        ("宁波", 29.87, 121.55), ("温州", 28.00, 120.67),
        ("珠海", 22.27, 113.58), ("佛山", 23.02, 113.12),
        ("东京", 35.68, 139.69), ("大阪", 34.69, 135.50),
        ("首尔", 37.57, 126.98), ("曼谷", 13.76, 100.50),
        ("胡志明市", 10.82, 106.63), ("新加坡", 1.35, 103.82),
        ("巴塞罗那", 41.39, 2.17), ("巴黎", 48.86, 2.35),
        ("纽约", 40.71, -74.01), ("伦敦", 51.51, -0.13),
    };

    private static string GetCityFromCoordinates(double lat, double lon)
    {
        string closest = "未知地区";
        double minDist = double.MaxValue;
        foreach (var city in KnownCities)
        {
            double dlat = lat - city.Lat;
            double dlon = lon - city.Lon;
            double dist = dlat * dlat + dlon * dlon;
            if (dist < minDist)
            {
                minDist = dist;
                closest = city.Name;
            }
        }
        return closest;
    }

    [RelayCommand]
    private async Task ShakeDiscover()
    {
        try
        {
            var allEntries = await _foodEntryRepo.GetAllAsync();
            if (allEntries.Count == 0) return;

            var entry = allEntries[new Random().Next(allEntries.Count)];
            ShakeResult = entry;
            ShakeStars = new string('★', entry.StarRating) + new string('☆', 5 - entry.StarRating);
            HasShakeResult = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] ShakeDiscover 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private void CloseShakeResult()
    {
        HasShakeResult = false;
    }

    [RelayCommand]
    private async Task ViewShakeDetail()
    {
        HasShakeResult = false;
        if (ShakeResult is null) return;
        try
        {
            await Shell.Current.GoToAsync($"EntryDetailPage?id={ShakeResult.Id}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] ViewShakeDetail 错误: {ex.Message}");
        }
    }
}
