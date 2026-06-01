using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// 稀有度统计（非数据库模型）
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
