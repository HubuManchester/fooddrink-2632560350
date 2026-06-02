using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.Models;

/// <summary>
/// YOLO 推理预测结果
/// </summary>
public partial class YoloPrediction : ObservableObject
{
    [ObservableProperty]
    private string _label = string.Empty;

    [ObservableProperty]
    private float _confidence;

    /// <summary>
    /// 边界框
    /// </summary>
    [ObservableProperty]
    private RectF _boundingBox;
}
