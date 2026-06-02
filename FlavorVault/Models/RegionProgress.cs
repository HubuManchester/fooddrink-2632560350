using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// Region collection progress (non-database model)
/// </summary>
public partial class RegionProgress : ObservableObject
{
    [ObservableProperty]
    private string _region = string.Empty;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _collectedCount;

    /// <summary>
    /// Progress text, format "X/Y"
    /// </summary>
    public string ProgressText => $"{CollectedCount}/{TotalCount}";

    /// <summary>
    /// Progress percentage (0~1, for ProgressBar binding)
    /// </summary>
    public double ProgressPercent => TotalCount > 0 ? (double)CollectedCount / TotalCount : 0;
}
