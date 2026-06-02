using SkiaSharp;

namespace FlavorVault.Services;

/// <summary>
/// 使用 SkiaSharp 进行图像预处理
/// 提供图像缩放、归一化和转换为浮点数组的功能
/// </summary>
public class SkiaImagePreprocessor : IDisposable
{
    private SKBitmap? _bitmap;

    /// <summary>
    /// 将图片缩放到目标尺寸（正方形），保持宽高比并填充黑色
    /// </summary>
    /// <param name="imagePath">图片文件路径</param>
    /// <param name="targetSize">目标尺寸（宽高相同），默认 640</param>
    /// <returns>缩放后的 SKBitmap，失败返回 null</returns>
    public SKBitmap? ResizeAndNormalize(string imagePath, int targetSize = 640)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                System.Diagnostics.Debug.WriteLine("[SkiaImagePreprocessor] 图片路径无效或文件不存在");
                return null;
            }

            // 释放上一次的 bitmap
            _bitmap?.Dispose();
            _bitmap = null;

            // 使用 SKCodec 解码，支持更多格式
            using var codec = SKCodec.Create(imagePath);
            if (codec == null)
            {
                System.Diagnostics.Debug.WriteLine("[SkiaImagePreprocessor] 无法创建图片解码器");
                return null;
            }

            // 获取原始图片信息
            var info = codec.Info;
            var originalWidth = info.Width;
            var originalHeight = info.Height;

            // 解码原始图片
            var original = SKBitmap.Decode(codec);
            if (original == null)
            {
                System.Diagnostics.Debug.WriteLine("[SkiaImagePreprocessor] 图片解码失败");
                return null;
            }

            using (original)
            {
                // 创建目标尺寸的 bitmap（正方形，黑色背景）
                _bitmap = new SKBitmap(targetSize, targetSize, SKColorType.Rgba8888, SKAlphaType.Premul);

                using var canvas = new SKCanvas(_bitmap);
                canvas.Clear(SKColors.Black);

                // 计算缩放比例，保持宽高比
                var scale = Math.Min((float)targetSize / originalWidth, (float)targetSize / originalHeight);
                var scaledWidth = (int)(originalWidth * scale);
                var scaledHeight = (int)(originalHeight * scale);

                // 居中绘制
                var offsetX = (targetSize - scaledWidth) / 2;
                var offsetY = (targetSize - scaledHeight) / 2;

                var destRect = new SKRect(offsetX, offsetY, offsetX + scaledWidth, offsetY + scaledHeight);
                canvas.DrawBitmap(original, destRect);

                return _bitmap;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SkiaImagePreprocessor] ResizeAndNormalize 异常: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 将 SKBitmap 转换为归一化浮点数组（0-1 范围，RGB 通道顺序，CHW 格式）
    /// 输出形状: [3, height, width]，用于 YOLO 模型输入
    /// </summary>
    /// <param name="bitmap">源位图</param>
    /// <returns>归一化浮点数组，失败返回空数组</returns>
    public float[] ToFloatArray(SKBitmap bitmap)
    {
        try
        {
            if (bitmap == null)
            {
                System.Diagnostics.Debug.WriteLine("[SkiaImagePreprocessor] bitmap 为 null");
                return [];
            }

            var width = bitmap.Width;
            var height = bitmap.Height;
            var totalPixels = width * height;

            // 输出大小: 3 通道 x height x width（CHW 格式）
            var result = new float[3 * totalPixels];

            // 读取像素数据
            var pixels = new SKColor[totalPixels];
            var ptr = bitmap.GetPixels();
            if (ptr == IntPtr.Zero)
            {
                // 降级方式：逐行读取
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixels[y * width + x] = bitmap.GetPixel(x, y);
                    }
                }
            }
            // 统一使用安全方式读取像素（避免 unsafe 代码）

            // 转换为 CHW 格式的归一化浮点数组
            // 通道顺序: R, G, B（YOLO 标准）
            for (int i = 0; i < totalPixels; i++)
            {
                var pixel = pixels[i];
                result[i] = pixel.Red / 255f;                              // R 通道
                result[totalPixels + i] = pixel.Green / 255f;              // G 通道
                result[2 * totalPixels + i] = pixel.Blue / 255f;           // B 通道
            }

            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SkiaImagePreprocessor] ToFloatArray 异常: {ex.Message}");
            return [];
        }
    }

    public void Dispose()
    {
        _bitmap?.Dispose();
        _bitmap = null;
    }
}
