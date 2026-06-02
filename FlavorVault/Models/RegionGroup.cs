using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// 地区分组
/// </summary>
public partial class RegionGroup : ObservableObject
{
    [ObservableProperty]
    private string _region = string.Empty;

    [ObservableProperty]
    private List<FoodEntry> _entries = new();
}
