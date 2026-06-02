using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlavorVault.Models;

namespace FlavorVault.ViewModels;

/// <summary>
/// Place item ViewModel, used for NearbyPlacesPage list item binding
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
    /// Create ViewModel from NearbyPlace model
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
    /// Return the first character based on place type
    /// </summary>
    private static string GetTypeInitial(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return "P";
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
            System.Diagnostics.Debug.WriteLine($"[PlaceItemViewModel] OpenMap error: {ex.Message}");
            try
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Notice", "Cannot open map app", "OK");
            }
            catch { }
        }
    }
}
