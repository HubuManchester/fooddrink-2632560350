using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// 地区收录进度（非数据库模型）
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
    /// 进度文本，格式 "X/Y"
    /// </summary>
    public string ProgressText => $"{CollectedCount}/{TotalCount}";

    /// <summary>
    /// 进度百分比（0~1，ProgressBar 绑定用）
    /// </summary>
    public double ProgressPercent => TotalCount > 0 ? (double)CollectedCount / TotalCount : 0;
}
