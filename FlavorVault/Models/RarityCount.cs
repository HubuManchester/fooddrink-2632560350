using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// Rarity statistics (non-database model)
/// </summary>
public partial class RarityCount : ObservableObject
{
    [ObservableProperty]
    private string _rarity = string.Empty;

    [ObservableProperty]
    private int _count;

    [ObservableProperty]
    private string _color = string.Empty;
}
