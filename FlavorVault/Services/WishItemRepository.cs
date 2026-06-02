using SQLite;
using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// Wish item repository
/// </summary>
public class WishItemRepository
{
    private readonly DatabaseService _dbService;

    public WishItemRepository(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    /// <summary>
    /// Get all wish items
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
    /// Get a wish item by ID
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
    /// Get wish items by priority
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
    /// Get uncompleted wish items
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
    /// Save a wish item (InsertOrReplace)
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
    /// Delete a wish item
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
    /// Mark a wish item as completed
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
    /// Get the total count of wish items
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
    /// Get the count of completed wish items
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
    /// Insert a wish item
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
    /// Update a wish item
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
