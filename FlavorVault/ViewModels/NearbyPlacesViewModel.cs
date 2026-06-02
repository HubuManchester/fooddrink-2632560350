using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// Nearby food places ViewModel, supports geolocation, place list and map navigation
/// </summary>
public partial class NearbyPlacesViewModel : BaseViewModel
{
    private readonly GeolocationService _geolocationService;
    private readonly PlaceMarkRepository _placeMarkRepo;

    [ObservableProperty]
    private string _currentLocation = "Locating...";

    [ObservableProperty]
    private ObservableCollection<NearbyPlace> _nearbyPlaces = new();

    [ObservableProperty]
    private int _placeCount;

    /// <summary>
    /// Coordinates text (for XAML binding, alias of CurrentLocation)
    /// </summary>
    public string CoordinatesText => CurrentLocation;

    /// <summary>
    /// Location status text (for XAML binding, alias of CurrentLocation)
    /// </summary>
    public string LocationStatus => CurrentLocation;

    /// <summary>
    /// Place count text
    /// </summary>
    public string PlaceCountText => $"{PlaceCount} food landmarks";

    /// <summary>
    /// Place list (for XAML binding, alias of NearbyPlaces)
    /// </summary>
    public ObservableCollection<NearbyPlace> Places => NearbyPlaces;

    public NearbyPlacesViewModel(
        GeolocationService geolocationService,
        PlaceMarkRepository placeMarkRepo)
    {
        _geolocationService = geolocationService;
        _placeMarkRepo = placeMarkRepo;
        Title = "Nearby Food Landmarks";
    }

    [RelayCommand]
    private async Task Load()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            // Get current location
            try
            {
                var location = await _geolocationService.GetCurrentLocationAsync();
                if (location is not null)
                {
                    CurrentLocation = $"Lat {location.Latitude:F6} Lon {location.Longitude:F6}";
                }
                else
                {
                    CurrentLocation = "Cannot get location";
                }
                OnPropertyChanged(nameof(CoordinatesText));
                OnPropertyChanged(nameof(LocationStatus));
            }
            catch (Exception)
            {
                CurrentLocation = "Location not authorized";
            }

            // Get nearby places
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
            System.Diagnostics.Debug.WriteLine($"[NearbyPlacesViewModel] Load error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[NearbyPlacesViewModel] OpenMap error: {ex.Message}");
            try
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Notice", "Cannot open map app", "OK");
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
