using SQLite;
using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// User preference KV storage repository
/// </summary>
public class UserProfileRepository
{
    private readonly DatabaseService _dbService;

    public UserProfileRepository(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    /// <summary>
    /// Get a preference value
    /// </summary>
    public async Task<string?> GetAsync(string key)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            var pref = await conn.Table<UserPreference>()
                .Where(p => p.Key == key)
                .FirstOrDefaultAsync();
            return pref?.Value;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UserProfileRepository] GetAsync error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Set a preference value
    /// </summary>
    public async Task SetAsync(string key, string value)
    {
        try
        {
            await _dbService.Init();
            var conn = _dbService.GetConnection();
            var existing = await conn.Table<UserPreference>()
                .Where(p => p.Key == key)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                existing.Value = value;
                await conn.UpdateAsync(existing);
            }
            else
            {
                await conn.InsertAsync(new UserPreference { Key = key, Value = value });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UserProfileRepository] SetAsync error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get the theme setting
    /// </summary>
    public async Task<string> GetThemeAsync()
    {
        var value = await GetAsync("theme");
        return value ?? "Light";
    }

    /// <summary>
    /// Set the theme
    /// </summary>
    public async Task SetThemeAsync(string theme)
    {
        await SetAsync("theme", theme);
    }

    /// <summary>
    /// Get the font size setting
    /// </summary>
    public async Task<string> GetFontSizeAsync()
    {
        var value = await GetAsync("fontSize");
        return value ?? "Medium";
    }

    /// <summary>
    /// Set the font size
    /// </summary>
    public async Task SetFontSizeAsync(string fontSize)
    {
        await SetAsync("fontSize", fontSize);
    }

    /// <summary>
    /// Get the username
    /// </summary>
    public async Task<string> GetUserNameAsync()
    {
        var value = await GetAsync("userName");
        return value ?? string.Empty;
    }

    /// <summary>
    /// Set the username
    /// </summary>
    public async Task SetUserNameAsync(string userName)
    {
        await SetAsync("userName", userName);
    }

    /// <summary>
    /// Check if this is the first run
    /// </summary>
    public async Task<bool> IsFirstRunAsync()
    {
        var value = await GetAsync("first_run");
        return value != "completed";
    }

    /// <summary>
    /// Mark first run as completed
    /// </summary>
    public async Task MarkFirstRunCompletedAsync()
    {
        await SetAsync("first_run", "completed");
    }
}
