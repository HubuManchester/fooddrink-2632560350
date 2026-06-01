using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

public partial class ExploreViewModel : BaseViewModel
{
    private readonly FoodEntryRepository _foodEntryRepo;
    private readonly PlaceMarkRepository _placeMarkRepo;
    private readonly SensorService _sensorService;
    private readonly GeolocationService _geolocationService;
    private readonly CatalogCalculator _calculator;
    private readonly HapticService _hapticService;
    private readonly WishItemRepository _wishItemRepo;

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

    public ObservableCollection<RegionGroup> Regions => RegionGroups;

    public string CoordinatesText => CurrentLocation;

    public string CompassText => $"{CompassDirection} {CompassHeading:F0}°";

    public string LocationStatus => CurrentLocation;

    public bool HasRandomEntry => ShakeResult is not null;

    public FoodEntry? RandomEntry => ShakeResult;

    public ExploreViewModel(
        FoodEntryRepository foodEntryRepo,
        PlaceMarkRepository placeMarkRepo,
        SensorService sensorService,
        GeolocationService geolocationService,
        CatalogCalculator calculator,
        HapticService hapticService,
        WishItemRepository wishItemRepo)
    {
        _foodEntryRepo = foodEntryRepo;
        _placeMarkRepo = placeMarkRepo;
        _sensorService = sensorService;
        _geolocationService = geolocationService;
        _calculator = calculator;
        _hapticService = hapticService;
        _wishItemRepo = wishItemRepo;
        Title = "探索";
    }

    [RelayCommand]
    private async Task Load()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await LoadCurrentLocation();
            await LoadNearbyPlaces();
            await LoadRegionGroups();
            await LoadPlaceMarks();
            StartCompass();
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
    private async Task RandomPick()
    {
        try
        {
            var allEntries = await _foodEntryRepo.GetAllAsync();
            if (allEntries.Count == 0) return;

            var random = new Random();
            ShakeResult = allEntries[random.Next(allEntries.Count)];
            OnPropertyChanged(nameof(HasRandomEntry));
            OnPropertyChanged(nameof(RandomEntry));

            await _hapticService.LightAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] RandomPick 错误: {ex.Message}");
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
            var existing = await _wishItemRepo.GetAllAsync();
            if (existing.Any(w => w.FoodName == entry.Name))
            {
                await Shell.Current.DisplayAlert("提示", $"「{entry.Name}」已在心愿清单中", "确定");
                return;
            }

            var wish = new WishItem
            {
                FoodName = entry.Name,
                Region = entry.Region,
                Priority = "有空就去",
                SourceReason = $"来自随机探索: {entry.CatalogNumber}"
            };
            await _wishItemRepo.SaveAsync(wish);
            await _hapticService.SuccessAsync();
            await Shell.Current.DisplayAlert("已添加", $"「{entry.Name}」已加入心愿清单", "确定");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] AddToWish 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private void StartCompass()
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

            _sensorService.ShakeDetected -= OnShakeDetected;
            _sensorService.StopShakeDetection();
            IsShaking = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ExploreViewModel] OnShakeDetected 错误: {ex.Message}");
        }
    }

    private void OnCompassChanged(object? sender, double heading)
    {
        CompassHeading = heading;
        CompassDirection = GetDirectionName(heading);
        OnPropertyChanged(nameof(CompassText));
    }

    private static string GetDirectionName(double heading)
    {
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
