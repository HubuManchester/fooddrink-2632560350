using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// 口味统计（非数据库模型）
/// </summary>
public partial class TasteStat : ObservableObject
{
    [ObservableProperty]
    private string _primaryTaste = string.Empty;

    [ObservableProperty]
    private int _count;
}
