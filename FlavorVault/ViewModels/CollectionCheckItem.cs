using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.ViewModels;

/// <summary>
/// 收藏集勾选项（用于 EntryEditPage 的收藏集多选列表）
/// </summary>
public partial class CollectionCheckItem : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private bool _isChecked;

    /// <summary>
    /// 收藏集名称（XAML 绑定用，Name 的别名）
    /// </summary>
    public string CollectionName => Name;
}
