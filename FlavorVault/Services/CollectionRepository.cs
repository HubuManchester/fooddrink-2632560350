using SQLite;
using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// 收藏集仓库
/// </summary>
public class CollectionRepository
{
    private readonly DatabaseService _dbService;

    public CollectionRepository(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    /// <summary>
    /// 获取所有收藏集
    /// </summary>
    public async Task<List<Collection>> GetAllAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<Collection>().OrderBy(c => c.SortOrder).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionRepository] GetAllAsync error: {ex.Message}");
            return new List<Collection>();
        }
    }

    /// <summary>
    /// 根据 ID 获取收藏集
    /// </summary>
    public async Task<Collection?> GetByIdAsync(int id)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<Collection>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionRepository] GetByIdAsync error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 根据名称获取收藏集
    /// </summary>
    public async Task<Collection?> GetByNameAsync(string name)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<Collection>().Where(c => c.Name == name).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionRepository] GetByNameAsync error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 保存收藏集（InsertOrReplace）
    /// </summary>
    public async Task<int> SaveAsync(Collection collection)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            collection.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (collection.Id == 0)
            {
                collection.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return await conn.InsertOrReplaceAsync(collection);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionRepository] SaveAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 删除收藏集
    /// </summary>
    public async Task<int> DeleteAsync(Collection collection)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.DeleteAsync(collection);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionRepository] DeleteAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 根据收藏集名称查询关联的食物条目
    /// </summary>
    public async Task<List<FoodEntry>> GetEntriesByCollectionNameAsync(string collectionName)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<FoodEntry>()
                .Where(e => e.CollectionName == collectionName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionRepository] GetEntriesByCollectionNameAsync error: {ex.Message}");
            return new List<FoodEntry>();
        }
    }

    /// <summary>
    /// 获取收藏集总数
    /// </summary>
    public async Task<int> GetCountAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<Collection>().CountAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionRepository] GetCountAsync error: {ex.Message}");
            return 0;
        }
    }
}
