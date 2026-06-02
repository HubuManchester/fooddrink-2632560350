using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// Nearby food place (non-database model)
/// </summary>
public partial class NearbyPlace : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _address = string.Empty;

    /// <summary>
    /// Distance (unit: km)
    /// </summary>
    [ObservableProperty]
    private double _distance;

    /// <summary>
    /// Place type abbreviation
    /// </summary>
    [ObservableProperty]
    private string _type = string.Empty;

    /// <summary>
    /// Distance text, format "X.X km"
    /// </summary>
    public string DistanceText => $"{Distance:F1} km";

    /// <summary>
    /// Coordinates text, format "Lat xx.xxxxxx Lon xx.xxxxxx"
    /// </summary>
    public string CoordinatesText =>
        Location != null
            ? $"Lat{Location.Latitude:F6} Lon{Location.Longitude:F6}"
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
