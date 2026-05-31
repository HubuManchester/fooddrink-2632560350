using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// 月度统计（非数据库模型）
/// </summary>
public partial class MonthlyCount : ObservableObject
{
    [ObservableProperty]
    private string _month = string.Empty;

    [ObservableProperty]
    private int _count;
}
