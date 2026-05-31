using SQLite;
using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// 心愿条目仓库
/// </summary>
public class WishItemRepository
{
    private readonly DatabaseService _dbService;

    public WishItemRepository(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    /// <summary>
    /// 获取所有心愿条目
    /// </summary>
    public async Task<List<WishItem>> GetAllAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<WishItem>().OrderByDescending(w => w.CreatedAt).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishItemRepository] GetAllAsync error: {ex.Message}");
            return new List<WishItem>();
        }
    }

    /// <summary>
    /// 根据 ID 获取心愿条目
    /// </summary>
    public async Task<WishItem?> GetByIdAsync(int id)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<WishItem>().Where(w => w.Id == id).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishItemRepository] GetByIdAsync error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 按优先级获取心愿条目
    /// </summary>
    public async Task<List<WishItem>> GetByPriorityAsync(string priority)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<WishItem>().Where(w => w.Priority == priority).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishItemRepository] GetByPriorityAsync error: {ex.Message}");
            return new List<WishItem>();
        }
    }

    /// <summary>
    /// 获取未完成的心愿条目
    /// </summary>
    public async Task<List<WishItem>> GetUncompletedAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<WishItem>().Where(w => w.IsCompleted == false).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishItemRepository] GetUncompletedAsync error: {ex.Message}");
            return new List<WishItem>();
        }
    }

    /// <summary>
    /// 保存心愿条目（InsertOrReplace）
    /// </summary>
    public async Task<int> SaveAsync(WishItem item)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            if (item.Id == 0)
            {
                item.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return await conn.InsertOrReplaceAsync(item);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishItemRepository] SaveAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 删除心愿条目
    /// </summary>
    public async Task<int> DeleteAsync(WishItem item)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.DeleteAsync(item);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishItemRepository] DeleteAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 标记心愿为已完成
    /// </summary>
    public async Task<int> MarkCompletedAsync(int id)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            var item = await conn.Table<WishItem>().Where(w => w.Id == id).FirstOrDefaultAsync();
            if (item != null)
            {
                item.IsCompleted = true;
                return await conn.UpdateAsync(item);
            }
            return 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishItemRepository] MarkCompletedAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 获取心愿条目总数
    /// </summary>
    public async Task<int> GetCountAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<WishItem>().CountAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishItemRepository] GetCountAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 获取已完成的心愿条目数
    /// </summary>
    public async Task<int> GetCompletedCountAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<WishItem>().Where(w => w.IsCompleted == true).CountAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishItemRepository] GetCompletedCountAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 插入心愿条目
    /// </summary>
    public async Task<int> InsertAsync(WishItem item)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            item.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return await conn.InsertAsync(item);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishItemRepository] InsertAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 更新心愿条目
    /// </summary>
    public async Task<int> UpdateAsync(WishItem item)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.UpdateAsync(item);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishItemRepository] UpdateAsync error: {ex.Message}");
            return 0;
        }
    }
}
