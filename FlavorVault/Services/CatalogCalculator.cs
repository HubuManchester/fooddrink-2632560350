using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// Catalog statistics calculation service
/// </summary>
public class CatalogCalculator
{
    /// <summary>
    /// All regions list
    /// </summary>
    private static readonly string[] AllRegions =
    {
        "Sichuan", "Cantonese", "Jiangnan", "Northern", "Northwest",
        "Japan", "Korea", "SoutheastAsia", "Western"
    };

    /// <summary>
    /// Rarity colors
    /// </summary>
    private static readonly (string Rarity, string Color)[] RarityColors =
    {
        ("Common", "#9E9E9E"),
        ("Recommended", "#42A5F5"),
        ("Limited", "#AB47BC"),
        ("Premium", "#FFB300")
    };

    /// <summary>
    /// Calculate region collection progress
    /// </summary>
    /// <param name="entries">All catalog entries list</param>
    /// <returns>Collection progress list for each region</returns>
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
                var collectedCount = group?.Count(e => e.CollectStatus == "Collected") ?? 0;

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
    /// Calculate rarity statistics
    /// </summary>
    /// <param name="entries">All catalog entries list</param>
    /// <returns>Rarity statistics list</returns>
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
    /// Calculate taste distribution statistics
    /// </summary>
    /// <param name="entries">All catalog entries list</param>
    /// <returns>Taste statistics list</returns>
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
