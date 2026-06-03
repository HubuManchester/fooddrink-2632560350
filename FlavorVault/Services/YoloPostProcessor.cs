using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// YOLO 输出后处理器
/// 实现 NMS（非极大值抑制）和坐标还原到原始图像尺寸
/// </summary>
public class YoloPostProcessor
{
    // YOLO 类别标签（食物相关，可根据实际模型调整）
    private static readonly string[] ClassLabels =
    [
        "hotdog", "pizza", "donut", "cake", "banana", "apple", "sandwich",
        "orange", "broccoli", "carrot", "bowl", "dining_table", "food"
    ];

    /// <summary>
    /// 处理 YOLO 模型输出
    /// </summary>
    /// <param name="output">模型原始输出数据</param>
    /// <param name="originalWidth">原始图像宽度</param>
    /// <param name="originalHeight">原始图像高度</param>
    /// <param name="confidenceThreshold">置信度阈值，默认 0.5</param>
    /// <returns>过滤后的预测结果列表</returns>
    public List<YoloPrediction> Process(float[] output, int originalWidth, int originalHeight, float confidenceThreshold = 0.5f)
    {
        try
        {
            if (output == null || output.Length == 0)
            {
                return [];
            }

            var predictions = new List<YoloPrediction>();

            // YOLOv8 输出格式: [1, (4 + numClasses), numDetections]
            // 前4个值: cx, cy, w, h（中心坐标 + 宽高，归一化到 0-1）
            // 之后是各类别置信度
            var numClasses = ClassLabels.Length;
            var valuesPerDetection = 4 + numClasses;

            // 计算检测数量
            int numDetections = output.Length / valuesPerDetection;

            var scaleX = (float)originalWidth;
            var scaleY = (float)originalHeight;

            for (int i = 0; i < numDetections; i++)
            {
                var offset = i * valuesPerDetection;

                // 解析边界框（中心坐标 + 宽高）
                var cx = output[offset];
                var cy = output[offset + 1];
                var w = output[offset + 2];
                var h = output[offset + 3];

                // 找到最大类别置信度
                float maxConf = 0;
                int maxClassIdx = 0;
                for (int c = 0; c < numClasses; c++)
                {
                    var conf = output[offset + 4 + c];
                    if (conf > maxConf)
                    {
                        maxConf = conf;
                        maxClassIdx = c;
                    }
                }

                // 过滤低置信度检测
                if (maxConf < confidenceThreshold)
                    continue;

                // 转换为左上角 + 宽高格式，并还原到原始图像尺寸
                var left = (cx - w / 2) * scaleX;
                var top = (cy - h / 2) * scaleY;
                var width = w * scaleX;
                var height = h * scaleY;

                // 确保坐标不超出图像边界
                left = Math.Max(0, Math.Min(left, scaleX));
                top = Math.Max(0, Math.Min(top, scaleY));
                width = Math.Max(0, Math.Min(width, scaleX - left));
                height = Math.Max(0, Math.Min(height, scaleY - top));

                predictions.Add(new YoloPrediction
                {
                    Label = maxClassIdx < ClassLabels.Length ? ClassLabels[maxClassIdx] : $"class_{maxClassIdx}",
                    Confidence = maxConf,
                    BoundingBox = new RectF(left, top, width, height)
                });
            }

            // 应用 NMS（非极大值抑制）
            return ApplyNMS(predictions, 0.45f);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[YoloPostProcessor] 处理异常: {ex.Message}");
            return [];
        }
    }

    /// <summary>
    /// 非极大值抑制（NMS）
    /// </summary>
    /// <param name="predictions">候选预测列表</param>
    /// <param name="iouThreshold">IoU 阈值，默认 0.45</param>
    /// <returns>过滤后的预测列表</returns>
    private List<YoloPrediction> ApplyNMS(List<YoloPrediction> predictions, float iouThreshold)
    {
        try
        {
            if (predictions.Count == 0)
                return predictions;

            // 按置信度降序排序
            var sorted = predictions.OrderByDescending(p => p.Confidence).ToList();
            var selected = new List<YoloPrediction>();

            while (sorted.Count > 0)
            {
                // 取置信度最高的预测
                var best = sorted[0];
                selected.Add(best);
                sorted.RemoveAt(0);

                // 移除与其 IoU 过高的预测
                sorted.RemoveAll(p =>
                {
                    var iou = CalculateIoU(best.BoundingBox, p.BoundingBox);
                    return iou > iouThreshold;
                });
            }

            return selected;
        }
        catch (Exception)
        {
            return predictions;
        }
    }

    /// <summary>
    /// 计算两个矩形的 IoU（交并比）
    /// </summary>
    private static float CalculateIoU(RectF a, RectF b)
    {
        var intersectLeft = Math.Max(a.Left, b.Left);
        var intersectTop = Math.Max(a.Top, b.Top);
        var intersectRight = Math.Min(a.Right, b.Right);
        var intersectBottom = Math.Min(a.Bottom, b.Bottom);

        if (intersectRight <= intersectLeft || intersectBottom <= intersectTop)
            return 0f;

        var intersectionArea = (intersectRight - intersectLeft) * (intersectBottom - intersectTop);
        var unionArea = a.Width * a.Height + b.Width * b.Height - intersectionArea;

        if (unionArea <= 0)
            return 0f;

        return intersectionArea / unionArea;
    }
}
