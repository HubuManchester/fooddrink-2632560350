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
    /// Whether refreshing (for XAML binding)
    /// </summary>
    [ObservableProperty]
    private bool _isRefreshing;

    /// <summary>
    /// Entry count text (for XAML binding)
    /// </summary>
    public string EntryCountText => $"{TotalCount} dishes collected";

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
        Title = "My Catalog";
    }

    [RelayCommand]
    private async Task LoadData()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var allEntries = await _foodEntryRepo.GetAllAsync();

            // Total count
            TotalCount = allEntries.Count;
            OnPropertyChanged(nameof(EntryCountText));

            // Region progress
            var progresses = _calculator.CalculateRegionProgress(allEntries);
            RegionProgresses.Clear();
            foreach (var p in progresses)
                RegionProgresses.Add(p);

            // Rarity stats
            var rarities = _calculator.CalculateRarityStats(allEntries);
            RarityStats.Clear();
            foreach (var r in rarities)
                RarityStats.Add(r);

            // Showcase picks (IsShowcase=true, max 5)
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
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] LoadData error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToCamera error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToCatalog error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToShowcaseManage error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToNewEntry error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToEntryDetail error: {ex.Message}");
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
                LocationCoordinates = $"Lat {location.Latitude:F6}  Lon {location.Longitude:F6}";
            }
            else
            {
                CurrentLocation = null;
                LocationCity = "Location unavailable";
                LocationCoordinates = "Check location permissions";
            }
            HasLocationInfo = true;
        }
        catch (Exception)
        {
            LocationCity = "Location failed";
            LocationCoordinates = "Check location permissions";
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
            await Shell.Current.DisplayAlert("Notice", "Cannot open map app", "OK");
        }
    }

    [RelayCommand]
    private void CloseLocationInfo()
    {
        HasLocationInfo = false;
    }

    private static readonly (string Name, double Lat, double Lon)[] KnownCities = new[]
    {
        ("Chengdu", 30.57, 104.07), ("Chongqing", 29.56, 106.55),
        ("Guangzhou", 23.13, 113.26), ("Shenzhen", 22.55, 114.06), ("Hong Kong", 22.32, 114.17),
        ("Shanghai", 31.23, 121.47), ("Hangzhou", 30.27, 120.15), ("Nanjing", 32.06, 118.80),
        ("Beijing", 39.90, 116.40), ("Tianjin", 39.08, 117.20),
        ("Xi'an", 34.26, 108.94), ("Lanzhou", 36.06, 103.83),
        ("Wuhan", 30.59, 114.30), ("Changsha", 28.23, 112.94),
        ("Chengdu", 30.57, 104.07), ("Kunming", 25.04, 102.68),
        ("Harbin", 45.75, 126.65), ("Shenyang", 41.80, 123.43),
        ("Dalian", 38.91, 121.60), ("Qingdao", 36.07, 120.38),
        ("Xiamen", 24.48, 118.09), ("Fuzhou", 26.07, 119.30),
        ("Zhengzhou", 34.75, 113.65), ("Jinan", 36.65, 116.99),
        ("Hefei", 31.82, 117.23), ("Nanchang", 28.68, 115.86),
        ("Guiyang", 26.65, 106.63), ("Nanning", 22.82, 108.37),
        ("Haikou", 20.04, 110.35), ("Sanya", 18.25, 109.50),
        ("Lhasa", 29.65, 91.10), ("Urumqi", 43.83, 87.62),
        ("Hohhot", 40.84, 111.75), ("Shijiazhuang", 38.04, 114.51),
        ("Taiyuan", 37.87, 112.55), ("Changchun", 43.88, 125.32),
        ("Suzhou", 31.30, 120.62), ("Wuxi", 31.49, 120.31),
        ("Ningbo", 29.87, 121.55), ("Wenzhou", 28.00, 120.67),
        ("Zhuhai", 22.27, 113.58), ("Foshan", 23.02, 113.12),
        ("Tokyo", 35.68, 139.69), ("Osaka", 34.69, 135.50),
        ("Seoul", 37.57, 126.98), ("Bangkok", 13.76, 100.50),
        ("Ho Chi Minh City", 10.82, 106.63), ("Singapore", 1.35, 103.82),
        ("Barcelona", 41.39, 2.17), ("Paris", 48.86, 2.35),
        ("New York", 40.71, -74.01), ("London", 51.51, -0.13),
    };

    private static string GetCityFromCoordinates(double lat, double lon)
    {
        string closest = "Unknown Area";
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
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] ShakeDiscover error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] ViewShakeDetail error: {ex.Message}");
        }
    }
}
