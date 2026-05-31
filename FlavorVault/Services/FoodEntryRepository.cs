using SQLite;
using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// 食物图鉴条目仓库
/// </summary>
public class FoodEntryRepository
{
    private readonly DatabaseService _dbService;

    public FoodEntryRepository(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    /// <summary>
    /// 获取所有图鉴条目
    /// </summary>
    public async Task<List<FoodEntry>> GetAllAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<FoodEntry>().OrderByDescending(e => e.CreatedAt).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] GetAllAsync error: {ex.Message}");
            return new List<FoodEntry>();
        }
    }

    /// <summary>
    /// 根据 ID 获取条目
    /// </summary>
    public async Task<FoodEntry?> GetByIdAsync(int id)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<FoodEntry>().Where(e => e.Id == id).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] GetByIdAsync error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 按地区获取条目
    /// </summary>
    public async Task<List<FoodEntry>> GetByRegionAsync(string region)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<FoodEntry>().Where(e => e.Region == region).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] GetByRegionAsync error: {ex.Message}");
            return new List<FoodEntry>();
        }
    }

    /// <summary>
    /// 按稀有度获取条目
    /// </summary>
    public async Task<List<FoodEntry>> GetByRarityAsync(string rarity)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<FoodEntry>().Where(e => e.Rarity == rarity).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] GetByRarityAsync error: {ex.Message}");
            return new List<FoodEntry>();
        }
    }

    /// <summary>
    /// 按主味获取条目
    /// </summary>
    public async Task<List<FoodEntry>> GetByTasteAsync(string taste)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<FoodEntry>().Where(e => e.PrimaryTaste == taste).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] GetByTasteAsync error: {ex.Message}");
            return new List<FoodEntry>();
        }
    }

    /// <summary>
    /// 获取橱窗展示条目（最多5个）
    /// </summary>
    public async Task<List<FoodEntry>> GetShowcaseAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<FoodEntry>()
                .Where(e => e.IsShowcase == true)
                .OrderByDescending(e => e.StarRating)
                .Take(5)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] GetShowcaseAsync error: {ex.Message}");
            return new List<FoodEntry>();
        }
    }

    /// <summary>
    /// 按名称搜索条目（模糊匹配）
    /// </summary>
    public async Task<List<FoodEntry>> SearchAsync(string keyword)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            var all = await conn.Table<FoodEntry>().ToListAsync();
            return all.Where(e => e.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] SearchAsync error: {ex.Message}");
            return new List<FoodEntry>();
        }
    }

    /// <summary>
    /// 保存条目（InsertOrReplace）
    /// </summary>
    public async Task<int> SaveAsync(FoodEntry entry)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            entry.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (entry.Id == 0)
            {
                entry.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return await conn.InsertOrReplaceAsync(entry);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] SaveAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 删除条目
    /// </summary>
    public async Task<int> DeleteAsync(FoodEntry entry)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.DeleteAsync(entry);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] DeleteAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 获取条目总数
    /// </summary>
    public async Task<int> GetCountAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<FoodEntry>().CountAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] GetCountAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 随机获取一个条目
    /// </summary>
    public async Task<FoodEntry?> GetRandomAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            var all = await conn.Table<FoodEntry>().ToListAsync();
            if (all.Count == 0) return null;
            var random = new Random();
            var index = random.Next(all.Count);
            return all[index];
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] GetRandomAsync error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 生成图鉴编号，查询当前最大编号+1，格式 FV-XXXX
    /// </summary>
    public async Task<string> GenerateCatalogNumberAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            var all = await conn.Table<FoodEntry>().ToListAsync();

            int maxNumber = 0;
            foreach (var entry in all)
            {
                if (entry.CatalogNumber?.StartsWith("FV-") == true)
                {
                    var numStr = entry.CatalogNumber.Substring(3);
                    if (int.TryParse(numStr, out int num) && num > maxNumber)
                    {
                        maxNumber = num;
                    }
                }
            }

            int nextNumber = maxNumber + 1;
            return $"FV-{nextNumber:D4}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] GenerateCatalogNumberAsync error: {ex.Message}");
            return "FV-0001";
        }
    }

    /// <summary>
    /// 获取地区统计（每个地区的条目数）
    /// </summary>
    public async Task<Dictionary<string, int>> GetRegionStatsAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            var all = await conn.Table<FoodEntry>().ToListAsync();
            return all.GroupBy(e => e.Region)
                      .ToDictionary(g => g.Key, g => g.Count());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] GetRegionStatsAsync error: {ex.Message}");
            return new Dictionary<string, int>();
        }
    }

    /// <summary>
    /// 获取稀有度统计
    /// </summary>
    public async Task<Dictionary<string, int>> GetRarityStatsAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            var all = await conn.Table<FoodEntry>().ToListAsync();
            return all.GroupBy(e => e.Rarity)
                      .ToDictionary(g => g.Key, g => g.Count());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] GetRarityStatsAsync error: {ex.Message}");
            return new Dictionary<string, int>();
        }
    }

    /// <summary>
    /// 获取口味统计
    /// </summary>
    public async Task<Dictionary<string, int>> GetTasteStatsAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            var all = await conn.Table<FoodEntry>().Where(e => e.PrimaryTaste != null).ToListAsync();
            return all.GroupBy(e => e.PrimaryTaste!)
                      .ToDictionary(g => g.Key, g => g.Count());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] GetTasteStatsAsync error: {ex.Message}");
            return new Dictionary<string, int>();
        }
    }

    /// <summary>
    /// 插入新条目
    /// </summary>
    public async Task<int> InsertAsync(FoodEntry entry)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            entry.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            entry.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return await conn.InsertAsync(entry);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] InsertAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 更新已有条目
    /// </summary>
    public async Task<int> UpdateAsync(FoodEntry entry)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            entry.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return await conn.UpdateAsync(entry);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FoodEntryRepository] UpdateAsync error: {ex.Message}");
            return 0;
        }
    }
}
