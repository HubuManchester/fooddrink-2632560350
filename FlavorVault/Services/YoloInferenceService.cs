using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// YOLO ONNX 模型推理服务
/// 模型缺失时优雅降级，不崩溃，返回空列表
/// </summary>
public class YoloInferenceService
{
    private readonly string _modelPath;
    private readonly YoloPostProcessor _postProcessor;
    private bool _isModelAvailable;

    /// <summary>
    /// 模型文件是否可用
    /// </summary>
    public bool IsModelAvailable => _isModelAvailable;

    public YoloInferenceService(YoloPostProcessor postProcessor)
    {
        _postProcessor = postProcessor;
        _modelPath = Path.Combine(FileSystem.AppDataDirectory, "models", "food_model.onnx");

        CheckModelAvailability();
    }

    /// <summary>
    /// 检查模型文件是否存在
    /// </summary>
    private void CheckModelAvailability()
    {
        try
        {
            _isModelAvailable = File.Exists(_modelPath);
        }
        catch (Exception)
        {
            _isModelAvailable = false;
        }
    }

    /// <summary>
    /// 对图片执行 YOLO 推理
    /// </summary>
    /// <param name="imagePath">图片路径</param>
    /// <returns>预测结果列表，模型不可用或出错时返回空列表</returns>
    public async Task<List<YoloPrediction>> PredictAsync(string imagePath)
    {
        try
        {
            // 模型不存在直接返回空列表
            if (!_isModelAvailable)
            {
                System.Diagnostics.Debug.WriteLine("[YoloInferenceService] 模型文件不可用，跳过推理");
                return [];
            }

            // 检查图片文件是否存在
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                System.Diagnostics.Debug.WriteLine("[YoloInferenceService] 图片文件不存在，跳过推理");
                return [];
            }

            // 使用 SkiaSharp 预处理图像
            using var preprocessor = new SkiaImagePreprocessor();
            var bitmap = preprocessor.ResizeAndNormalize(imagePath, 640);
            if (bitmap == null)
            {
                System.Diagnostics.Debug.WriteLine("[YoloInferenceService] 图像预处理失败");
                return [];
            }

            var inputData = preprocessor.ToFloatArray(bitmap);
            if (inputData == null || inputData.Length == 0)
            {
                System.Diagnostics.Debug.WriteLine("[YoloInferenceService] 图像数据转换失败");
                return [];
            }

            // 加载 ONNX 模型并执行推理
            var predictions = await Task.Run(() =>
            {
                // 尝试使用 OnnxRuntime 进行推理
                // 如果 OnnxRuntime 不可用，降级返回空列表
                return RunOnnxInference(inputData, bitmap.Width, bitmap.Height);
            });

            return predictions;
        }
        catch (FileNotFoundException)
        {
            System.Diagnostics.Debug.WriteLine("[YoloInferenceService] 模型文件未找到");
            _isModelAvailable = false;
            return [];
        }
        catch (DllNotFoundException)
        {
            System.Diagnostics.Debug.WriteLine("[YoloInferenceService] ONNX Runtime 依赖缺失");
            _isModelAvailable = false;
            return [];
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[YoloInferenceService] 推理异常: {ex.Message}");
            return [];
        }
    }

    /// <summary>
    /// 执行 ONNX 模型推理（需要 Microsoft.ML.OnnxRuntime 包）
    /// 当前为占位实现，当 OnnxRuntime NuGet 包添加后可启用
    /// </summary>
    private List<YoloPrediction> RunOnnxInference(float[] inputData, int imageWidth, int imageHeight)
    {
        try
        {
            // 尝试动态加载 OnnxRuntime
            var sessionType = Type.GetType("Microsoft.ML.OnnxRuntime.InferenceSession, Microsoft.ML.OnnxRuntime");
            if (sessionType == null)
            {
                System.Diagnostics.Debug.WriteLine("[YoloInferenceService] OnnxRuntime 未安装，跳过推理");
                return [];
            }

            // 创建 InferenceSession
            var session = Activator.CreateInstance(sessionType, _modelPath);
            if (session == null)
            {
                return [];
            }

            // 创建输入张量
            var tensorType = Type.GetType("Microsoft.ML.OnnxRuntime.Tensors.DenseTensor`1[[System.Single, System.Private.CoreLib]], Microsoft.ML.OnnxRuntime");
            if (tensorType == null)
            {
                return [];
            }

            // 输入形状: [1, 3, 640, 640]（NCHW 格式）
            var dimensions = new[] { 1, 3, 640, 640 };
            var tensor = Activator.CreateInstance(tensorType, inputData, dimensions);
            if (tensor == null)
            {
                return [];
            }

            // 构造命名输入
            var namedValueType = Type.GetType("Microsoft.ML.OnnxRuntime.NamedOnnxValue, Microsoft.ML.OnnxRuntime");
            if (namedValueType == null)
            {
                return [];
            }

            var inputName = "images"; // YOLO 标准输入名称
            var namedValueMethod = namedValueType.GetMethod("CreateFromTensor", new[] { typeof(string), tensor!.GetType() });
            if (namedValueMethod == null)
            {
                return [];
            }

            // 使用动态方式调用以避免编译时依赖
            var namedValue = namedValueMethod.Invoke(null, new object[] { inputName, tensor });

            // 执行推理
            var runMethod = sessionType.GetMethod("Run", new[] { typeof(IEnumerable<>).MakeGenericType(namedValueType) });
            if (runMethod == null)
            {
                return [];
            }

            var inputs = Array.CreateInstance(namedValueType, 1);
            inputs.SetValue(namedValue, 0);

            var results = runMethod.Invoke(session, new object[] { inputs });
            if (results == null)
            {
                return [];
            }

            // 提取输出数据
            var resultsList = new List<object>();
            foreach (var item in (System.Collections.IEnumerable)results)
            {
                resultsList.Add(item);
            }

            if (resultsList.Count == 0)
            {
                return [];
            }

            // 从第一个输出中获取 float 数组
            var firstOutput = resultsList[0];
            var tensorProperty = firstOutput.GetType().GetProperty("Tensor");
            if (tensorProperty == null)
            {
                return [];
            }

            var outputTensor = tensorProperty.GetValue(firstOutput);
            if (outputTensor == null)
            {
                return [];
            }

            // 获取输出数据
            var toArrayMethod = outputTensor.GetType().GetMethod("ToArray");
            if (toArrayMethod == null)
            {
                return [];
            }

            var outputData = toArrayMethod.Invoke(outputTensor, null) as float[];
            if (outputData == null)
            {
                return [];
            }

            // 后处理
            return _postProcessor.Process(outputData, imageWidth, imageHeight, 0.5f);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[YoloInferenceService] ONNX 推理执行失败: {ex.Message}");
            return [];
        }
    }

    /// <summary>
    /// 将英文食物标签映射为中文名称
    /// </summary>
    public string? MapLabelToChinese(string label)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "sushi", "寿司" }, { "ramen", "拉面" }, { "tempura", "天妇罗" },
            { "dumpling", "饺子" }, { "noodle", "面条" }, { "rice", "米饭" },
            { "chicken", "鸡肉" }, { "beef", "牛肉" }, { "pork", "猪肉" },
            { "fish", "鱼" }, { "shrimp", "虾" }, { "soup", "汤" },
            { "bread", "面包" }, { "cake", "蛋糕" }, { "salad", "沙拉" },
            { "pizza", "披萨" }, { "burger", "汉堡" }, { "steak", "牛排" },
            { "tofu", "豆腐" }, { "curry", "咖喱" }, { "fried_rice", "炒饭" },
        };
        return map.GetValueOrDefault(label);
    }
}
