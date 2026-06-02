using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.ViewModels;

/// <summary>
/// Collection check item (used for the collection multi-select list on EntryEditPage)
/// </summary>
public partial class CollectionCheckItem : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isChecked;

    /// <summary>
    /// Collection name (for XAML binding, alias of Name)
    /// </summary>
    public string CollectionName => Name;
}
