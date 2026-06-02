using CommunityToolkit.Mvvm.ComponentModel;
using FlavorVault.Models;

namespace FlavorVault.ViewModels;

/// <summary>
/// Nearby place ViewModel, used for ExplorePage horizontal card binding
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
    /// Create ViewModel from NearbyPlace model
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
    /// Return FontAwesome icon Unicode based on place type
    /// </summary>
    private static string GetTypeIcon(string? type)
    {
        return type switch
        {
            "Restaurant" => "",
            "Snack" => "",
            "Market" => "",
            "Dessert" => "",
            "Drink" => "",
            _ => ""
        };
    }
}
