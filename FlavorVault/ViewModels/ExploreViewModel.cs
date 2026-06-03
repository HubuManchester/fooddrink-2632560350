using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// 探索页 ViewModel，支持摇一摇发现、指南针、附近地点和地区入口
/// </summary>
public partial class ExploreViewModel : BaseViewModel
{
    private readonly FoodEntryRepository _foodEntryRepo;
    private readonly PlaceMarkRepository _placeMarkRepo;
    private readonly SensorService _sensorService;
    private readonly GeolocationService _geolocationService;
    private readonly CatalogCalculator _calculator;
    private readonly HapticService _hapticService;

    [ObservableProperty]
    private string _currentLocation = "正在定位...";

    [ObservableProperty]
    private double _compassHeading;

    [ObservableProperty]
    private string _compassDirection = "北";

    [ObservableProperty]
    private FoodEntry? _shakeResult;

    [ObservableProperty]
    private bool _isShaking;

    [ObservableProperty]
    private ObservableCollection<NearbyPlace> _nearbyPlaces = new();

    [ObservableProperty]
    private ObservableCollection<RegionGroup> _regionGroups = new();

    [ObservableProperty]
    private ObservableCollection<PlaceMark> _displayPlaceMarks = new();

    /// <summary>
    /// 地区分组（XAML 绑定用，RegionGroups 的别名）
    /// </summary>
    public ObservableCollection<RegionGroup> Regions => RegionGroups;

    /// <summary>
    /// 坐标文本（XAML 绑定用，CurrentLocation 的别名）
    /// </summary>
    public string CoordinatesText => CurrentLocation;

    /// <summary>
    /// 指南针文本（XAML 绑定用）
    /// </summary>
    public string CompassText => $"{CompassDirection} {CompassHeading:F0}°";

    /// <summary>
    /// 位置状态文本（XAML 绑定用，CurrentLocation 的别名）
    /// </summary>
    public string LocationStatus => CurrentLocation;

    /// <summary>
    /// 是否有随机结果（XAML 绑定用，HasShakeResult 不存在，基于 ShakeResult）
    /// </summary>
    public bool HasRandomEntry => ShakeResult is not null;

    /// <summary>
    /// 随机条目（XAML 绑定用，ShakeResult 的别名）
    /// </summary>
    public FoodEntry? RandomEntry => ShakeResult;

    public ExploreViewModel(
        FoodEntryRepository foodEntryRepo,
        PlaceMarkRepository placeMarkRepo,
        SensorService sensorService,
        GeolocationService geolocationService,
        CatalogCalculator calculator,
        HapticService hapticService)
    {
        _foodEntryRepo = foodEntryRepo;
        _placeMarkRepo = placeMarkRepo;
        _sensorService = sensorService;
        _geolocationService = geolocationService;
        _calculator = calculator;
        _hapticService = hapticService;
        Title = "探索";
    }

    [RelayCommand]
    private async Task Load()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            // 加载当前位置
            await LoadCurrentLocation();

            // 加载附近地点
            await LoadNearbyPlaces();

            // 加载地区分组
            await LoadRegionGroups();

            // 加载地点标记
            await LoadPlaceMarks();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] Load 错误: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task StartShake()
    {
        if (IsShaking) return;
        IsShaking = true;
        try
        {
            _sensorService.ShakeDetected += OnShakeDetected;
            _sensorService.StartShakeDetection();
        }
        catch (Exception ex)
        {
            IsShaking = false;
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] StartShake 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private void StopShake()
    {
        try
        {
            _sensorService.ShakeDetected -= OnShakeDetected;
            _sensorService.StopShakeDetection();
            IsShaking = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] StopShake 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToNearbyPlaces()
    {
        try
        {
            await Shell.Current.GoToAsync("NearbyPlacesPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] GoToNearbyPlaces 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToRegionDetail(string? region)
    {
        if (string.IsNullOrWhiteSpace(region)) return;
        try
        {
            await Shell.Current.GoToAsync($"RegionDetailPage?region={Uri.EscapeDataString(region)}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] GoToRegionDetail 错误: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] GoToEntryDetail 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddToWish(FoodEntry? entry)
    {
        if (entry is null) return;
        try
        {
            // 导航到新增心愿（可预填名称）
            await Shell.Current.GoToAsync($"EntryEditPage?wishName={Uri.EscapeDataString(entry.Name)}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] AddToWish 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task StartCompass()
    {
        try
        {
            _sensorService.CompassChanged += OnCompassChanged;
            _sensorService.StartCompass();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] StartCompass 错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 摇一摇检测回调
    /// </summary>
    private async void OnShakeDetected(object? sender, EventArgs e)
    {
        try
        {
            var allEntries = await _foodEntryRepo.GetAllAsync();
            if (allEntries.Count == 0) return;

            var random = new Random();
            var index = random.Next(allEntries.Count);
            ShakeResult = allEntries[index];

            OnPropertyChanged(nameof(HasRandomEntry));
            OnPropertyChanged(nameof(RandomEntry));

            await _hapticService.SuccessAsync();

            // 停止监听
            _sensorService.ShakeDetected -= OnShakeDetected;
            _sensorService.StopShakeDetection();
            IsShaking = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] OnShakeDetected 错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 指南针方向变更回调
    /// </summary>
    private void OnCompassChanged(object? sender, double heading)
    {
        CompassHeading = heading;
        CompassDirection = GetDirectionName(heading);
        OnPropertyChanged(nameof(CompassText));
    }

    /// <summary>
    /// 根据角度获取方向名称
    /// </summary>
    private static string GetDirectionName(double heading)
    {
        // 归一化到 0-360
        var h = ((heading % 360) + 360) % 360;
        return h switch
        {
            >= 337.5 or < 22.5 => "北",
            >= 22.5 and < 67.5 => "东北",
            >= 67.5 and < 112.5 => "东",
            >= 112.5 and < 157.5 => "东南",
            >= 157.5 and < 202.5 => "南",
            >= 202.5 and < 247.5 => "西南",
            >= 247.5 and < 292.5 => "西",
            >= 292.5 and < 337.5 => "西北",
            _ => "北"
        };
    }

    /// <summary>
    /// 加载当前位置
    /// </summary>
    private async Task LoadCurrentLocation()
    {
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
        }
        catch (Exception)
        {
            CurrentLocation = "定位未授权";
        }
        OnPropertyChanged(nameof(CoordinatesText));
        OnPropertyChanged(nameof(LocationStatus));
    }

    /// <summary>
    /// 加载附近地点
    /// </summary>
    private async Task LoadNearbyPlaces()
    {
        try
        {
            var places = await _geolocationService.GetNearbyPlacesAsync();
            NearbyPlaces.Clear();
            foreach (var place in places.Take(4))
                NearbyPlaces.Add(place);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] LoadNearbyPlaces 错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 加载地区分组
    /// </summary>
    private async Task LoadRegionGroups()
    {
        try
        {
            var allEntries = await _foodEntryRepo.GetAllAsync();
            var groups = allEntries
                .GroupBy(e => e.Region)
                .Select(g => new RegionGroup { Region = g.Key, Entries = g.ToList() })
                .OrderBy(g => g.Region)
                .ToList();

            RegionGroups.Clear();
            foreach (var group in groups)
                RegionGroups.Add(group);
            OnPropertyChanged(nameof(Regions));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] LoadRegionGroups 错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 加载地点标记
    /// </summary>
    private async Task LoadPlaceMarks()
    {
        try
        {
            var marks = await _placeMarkRepo.GetAllAsync();
            DisplayPlaceMarks.Clear();
            foreach (var mark in marks)
                DisplayPlaceMarks.Add(mark);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] LoadPlaceMarks 错误: {ex.Message}");
        }
    }
}
