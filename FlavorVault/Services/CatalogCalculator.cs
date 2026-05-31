using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// 图鉴统计计算服务
/// </summary>
public class CatalogCalculator
{
    /// <summary>
    /// 全部地区列表
    /// </summary>
    private static readonly string[] AllRegions =
    {
        "川渝", "粤港", "江南", "北方", "西北",
        "日本", "韩国", "东南亚", "欧美"
    };

    /// <summary>
    /// 稀有度对应的颜色
    /// </summary>
    private static readonly (string Rarity, string Color)[] RarityColors =
    {
        ("日常", "#9E9E9E"),
        ("推荐", "#42A5F5"),
        ("限定", "#AB47BC"),
        ("珍藏", "#FFB300")
    };

    /// <summary>
    /// 计算各地区收录进度
    /// </summary>
    /// <param name="entries">所有图鉴条目列表</param>
    /// <returns>每个地区的收录进度列表</returns>
    public List<RegionProgress> CalculateRegionProgress(List<FoodEntry> entries)
    {
        try
        {
            var result = new List<RegionProgress>();

            var regionGroups = entries
                .GroupBy(e => e.Region)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var region in AllRegions)
            {
                regionGroups.TryGetValue(region, out var group);
                var totalCount = group?.Count ?? 0;
                var collectedCount = group?.Count(e => e.CollectStatus == "已收藏") ?? 0;

                result.Add(new RegionProgress
                {
                    Region = region,
                    TotalCount = totalCount,
                    CollectedCount = collectedCount
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CatalogCalculator] CalculateRegionProgress error: {ex.Message}");
            return new List<RegionProgress>();
        }
    }

    /// <summary>
    /// 计算稀有度统计
    /// </summary>
    /// <param name="entries">所有图鉴条目列表</param>
    /// <returns>稀有度统计列表</returns>
    public List<RarityCount> CalculateRarityStats(List<FoodEntry> entries)
    {
        try
        {
            var result = new List<RarityCount>();

            var rarityGroups = entries
                .GroupBy(e => e.Rarity)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var (rarity, color) in RarityColors)
            {
                rarityGroups.TryGetValue(rarity, out var count);

                result.Add(new RarityCount
                {
                    Rarity = rarity,
                    Count = count,
                    Color = color
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CatalogCalculator] CalculateRarityStats error: {ex.Message}");
            return new List<RarityCount>();
        }
    }

    /// <summary>
    /// 计算口味分布统计
    /// </summary>
    /// <param name="entries">所有图鉴条目列表</param>
    /// <returns>口味统计列表</returns>
    public List<TasteStat> CalculateTasteStats(List<FoodEntry> entries)
    {
        try
        {
            return entries
                .Where(e => !string.IsNullOrEmpty(e.PrimaryTaste))
                .GroupBy(e => e.PrimaryTaste!)
                .Select(g => new TasteStat
                {
                    PrimaryTaste = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(t => t.Count)
                .ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CatalogCalculator] CalculateTasteStats error: {ex.Message}");
            return new List<TasteStat>();
        }
    }
}
