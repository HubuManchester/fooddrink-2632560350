using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// Monthly statistics (non-database model)
/// </summary>
public partial class MonthlyCount : ObservableObject
{
    [ObservableProperty]
    private string _month = string.Empty;

    [ObservableProperty]
    private int _count;
}
