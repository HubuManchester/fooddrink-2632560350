using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using FlavorVault.Models;

namespace FlavorVault.Controls;

public partial class RegionPieChart : ContentView
{
    private static readonly (string Region, SKColor Color)[] RegionColors =
    {
        ("川渝", new SKColor(0xE5, 0x39, 0x35)),
        ("粤港", new SKColor(0xFF, 0x98, 0x00)),
        ("江南", new SKColor(0x4C, 0xAF, 0x50)),
        ("北方", new SKColor(0x21, 0x96, 0xF3)),
        ("西北", new SKColor(0x8D, 0x6E, 0x63)),
        ("日本", new SKColor(0xE9, 0x1E, 0x63)),
        ("韩国", new SKColor(0x9C, 0x27, 0xB0)),
        ("东南亚", new SKColor(0xFF, 0xC1, 0x07)),
        ("欧美", new SKColor(0x00, 0xBC, 0xD4)),
    };

    public static readonly BindableProperty RegionProgressesProperty =
        BindableProperty.Create(
            nameof(RegionProgresses),
            typeof(ObservableCollection<RegionProgress>),
            typeof(RegionPieChart),
            default(ObservableCollection<RegionProgress>),
            propertyChanged: OnRegionProgressesChanged);

    public ObservableCollection<RegionProgress> RegionProgresses
    {
        get => (ObservableCollection<RegionProgress>)GetValue(RegionProgressesProperty);
        set => SetValue(RegionProgressesProperty, value);
    }

    public RegionPieChart()
    {
        InitializeComponent();
    }

    private static void OnRegionProgressesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (RegionPieChart)bindable;

        if (oldValue is ObservableCollection<RegionProgress> oldCollection)
            oldCollection.CollectionChanged -= control.OnCollectionChanged;

        if (newValue is ObservableCollection<RegionProgress> newCollection)
            newCollection.CollectionChanged += control.OnCollectionChanged;

        control.CanvasView?.InvalidateSurface();
        control.BuildLegend();
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CanvasView?.InvalidateSurface();
        BuildLegend();
    }

    private void OnCanvasViewPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear();

        if (RegionProgresses == null || RegionProgresses.Count == 0)
            return;

        var info = e.Info;
        var cx = info.Width / 2f;
        var cy = info.Height / 2f;
        var size = Math.Min(info.Width, info.Height);
        var outerR = size / 2f - 12;
        var innerR = outerR * 0.58f;

        var activeRegions = RegionProgresses.Where(r => r.TotalCount > 0).ToList();
        var total = activeRegions.Sum(r => r.TotalCount);
        var colorMap = RegionColors.ToDictionary(r => r.Region, r => r.Color);
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

        if (total == 0)
        {
            // 空状态：灰色圆环
            using var emptyPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = isDark ? new SKColor(0x44, 0x44, 0x44) : new SKColor(0xE0, 0xE0, 0xE0),
                StrokeWidth = outerR - innerR,
                IsAntialias = true
            };
            canvas.DrawCircle(cx, cy, (outerR + innerR) / 2, emptyPaint);

            using var emptyFont = new SKFont { Size = 14 };
            using var emptyTextPaint = new SKPaint { IsAntialias = true, Color = isDark ? SKColors.LightGray : SKColors.Gray };
            var emptyText = "暂无数据";
            var emptyW = emptyFont.MeasureText(emptyText);
            canvas.DrawText(emptyText, cx - emptyW / 2, cy + 5, emptyFont, emptyTextPaint);
            return;
        }

        // 绘制甜甜圈切片
        float startAngle = -90f;
        foreach (var region in activeRegions)
        {
            var sweepAngle = 360f * region.TotalCount / total;
            if (!colorMap.TryGetValue(region.Region, out var color))
                color = SKColors.Gray;

            using var slicePaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = color,
                IsAntialias = true
            };

            using var path = new SKPath();
            var startRad = startAngle * Math.PI / 180;
            var endRad = (startAngle + sweepAngle) * Math.PI / 180;

            // 外弧起点
            path.MoveTo(
                cx + outerR * (float)Math.Cos(startRad),
                cy + outerR * (float)Math.Sin(startRad));

            // 外弧
            path.ArcTo(
                new SKRect(cx - outerR, cy - outerR, cx + outerR, cy + outerR),
                startAngle, sweepAngle, false);

            // 连线到内弧终点
            path.LineTo(
                cx + innerR * (float)Math.Cos(endRad),
                cy + innerR * (float)Math.Sin(endRad));

            // 内弧（反向）
            path.ArcTo(
                new SKRect(cx - innerR, cy - innerR, cx + innerR, cy + innerR),
                startAngle + sweepAngle, -sweepAngle, false);

            path.Close();
            canvas.DrawPath(path, slicePaint);

            startAngle += sweepAngle;
        }

        // 中心文字：总数
        var textColor = isDark ? new SKColor(0xFC, 0xE4, 0xEC) : new SKColor(0x88, 0x0E, 0x4F);

        using var bigFont = new SKFont { Size = 36 };
        using var bigPaint = new SKPaint { IsAntialias = true, Color = textColor };
        var totalText = total.ToString();
        var totalW = bigFont.MeasureText(totalText);
        canvas.DrawText(totalText, cx - totalW / 2, cy - 4, bigFont, bigPaint);

        using var smallFont = new SKFont { Size = 12 };
        var subColor = isDark ? new SKColor(0xF4, 0x8F, 0xB1) : new SKColor(0xE9, 0x1E, 0x63);
        using var smallPaint = new SKPaint { IsAntialias = true, Color = subColor };
        var subText = "道美食";
        var subW = smallFont.MeasureText(subText);
        canvas.DrawText(subText, cx - subW / 2, cy + 18, smallFont, smallPaint);
    }

    private void BuildLegend()
    {
        LegendContainer.Children.Clear();
        if (RegionProgresses == null) return;

        var activeRegions = RegionProgresses.Where(r => r.TotalCount > 0).ToList();
        var colorMap = RegionColors.ToDictionary(r => r.Region, r => r.Color);
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

        foreach (var region in activeRegions)
        {
            if (!colorMap.TryGetValue(region.Region, out var skColor))
                skColor = SKColors.Gray;

            var color = Color.FromRgba(skColor.Red, skColor.Green, skColor.Blue, skColor.Alpha);

            var item = new HorizontalStackLayout { Spacing = 6, Margin = new Thickness(0, 3) };

            var dot = new BoxView
            {
                Color = color,
                WidthRequest =10,
                HeightRequest = 10,
                CornerRadius = 2,
                VerticalOptions = LayoutOptions.Center
            };

            var nameLabel = new Label
            {
                Text = region.Region,
                FontSize = 12,
                TextColor = isDark ? Color.FromHex("#FCE4EC") : Color.FromHex("#880E4F"),
                VerticalOptions = LayoutOptions.Center
            };

            var countLabel = new Label
            {
                Text = region.TotalCount.ToString(),
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                TextColor = isDark ? Color.FromHex("#F48FB1") : Color.FromHex("#E91E63"),
                VerticalOptions = LayoutOptions.Center
            };

            item.Children.Add(dot);
            item.Children.Add(nameLabel);
            item.Children.Add(countLabel);
            LegendContainer.Children.Add(item);
        }

        // 无数据时显示提示
        if (activeRegions.Count == 0)
        {
            LegendContainer.Children.Add(new Label
            {
                Text = "暂无数据",
                FontSize = 12,
                TextColor = isDark ? Color.FromHex("#F48FB1") : Color.FromHex("#E91E63"),
                HorizontalOptions = LayoutOptions.Center
            });
        }
    }
}
