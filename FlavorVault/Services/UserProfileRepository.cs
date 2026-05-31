using SQLite;
using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// 用户偏好 KV 存储仓库
/// </summary>
public class UserProfileRepository
{
    private readonly DatabaseService _dbService;

    public UserProfileRepository(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    /// <summary>
    /// 获取偏好值
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
    /// 设置偏好值
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
    /// 获取主题设置
    /// </summary>
    public async Task<string> GetThemeAsync()
    {
        var value = await GetAsync("theme");
        return value ?? "Light";
    }

    /// <summary>
    /// 设置主题
    /// </summary>
    public async Task SetThemeAsync(string theme)
    {
        await SetAsync("theme", theme);
    }

    /// <summary>
    /// 获取字体大小设置
    /// </summary>
    public async Task<string> GetFontSizeAsync()
    {
        var value = await GetAsync("fontSize");
        return value ?? "Medium";
    }

    /// <summary>
    /// 设置字体大小
    /// </summary>
    public async Task SetFontSizeAsync(string fontSize)
    {
        await SetAsync("fontSize", fontSize);
    }

    /// <summary>
    /// 获取用户名
    /// </summary>
    public async Task<string> GetUserNameAsync()
    {
        var value = await GetAsync("userName");
        return value ?? string.Empty;
    }

    /// <summary>
    /// 设置用户名
    /// </summary>
    public async Task SetUserNameAsync(string userName)
    {
        await SetAsync("userName", userName);
    }

    /// <summary>
    /// 是否首次运行
    /// </summary>
    public async Task<bool> IsFirstRunAsync()
    {
        var value = await GetAsync("first_run");
        return value != "completed";
    }

    /// <summary>
    /// 标记首次运行已完成
    /// </summary>
    public async Task MarkFirstRunCompletedAsync()
    {
        await SetAsync("first_run", "completed");
    }
}
