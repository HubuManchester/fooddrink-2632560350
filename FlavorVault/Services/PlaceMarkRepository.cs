using SQLite;
using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// Place mark repository
/// </summary>
public class PlaceMarkRepository
{
    private readonly DatabaseService _dbService;

    public PlaceMarkRepository(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    /// <summary>
    /// Get all place marks
    /// </summary>
    public async Task<List<PlaceMark>> GetAllAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<PlaceMark>().OrderByDescending(p => p.CreatedAt).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PlaceMarkRepository] GetAllAsync error: {ex.Message}");
            return new List<PlaceMark>();
        }
    }

    /// <summary>
    /// Get a place mark by ID
    /// </summary>
    public async Task<PlaceMark?> GetByIdAsync(int id)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<PlaceMark>().Where(p => p.Id == id).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PlaceMarkRepository] GetByIdAsync error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get place marks by category
    /// </summary>
    public async Task<List<PlaceMark>> GetByCategoryAsync(string category)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<PlaceMark>().Where(p => p.Category == category).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PlaceMarkRepository] GetByCategoryAsync error: {ex.Message}");
            return new List<PlaceMark>();
        }
    }

    /// <summary>
    /// Get place marks by region
    /// </summary>
    public async Task<List<PlaceMark>> GetByRegionAsync(string region)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<PlaceMark>().Where(p => p.Region == region).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PlaceMarkRepository] GetByRegionAsync error: {ex.Message}");
            return new List<PlaceMark>();
        }
    }

    /// <summary>
    /// Get place marks sorted by rating (descending)
    /// </summary>
    public async Task<List<PlaceMark>> GetByRatingAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<PlaceMark>().OrderByDescending(p => p.StarRating).ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PlaceMarkRepository] GetByRatingAsync error: {ex.Message}");
            return new List<PlaceMark>();
        }
    }

    /// <summary>
    /// Save a place mark (InsertOrReplace)
    /// </summary>
    public async Task<int> SaveAsync(PlaceMark placeMark)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            if (placeMark.Id == 0)
            {
                placeMark.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return await conn.InsertOrReplaceAsync(placeMark);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PlaceMarkRepository] SaveAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// Delete a place mark
    /// </summary>
    public async Task<int> DeleteAsync(PlaceMark placeMark)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.DeleteAsync(placeMark);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PlaceMarkRepository] DeleteAsync error: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// Get the total count of place marks
    /// </summary>
    public async Task<int> GetCountAsync()
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            return await conn.Table<PlaceMark>().CountAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PlaceMarkRepository] GetCountAsync error: {ex.Message}");
            return 0;
        }
    }
}
