using SQLite;
using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// SQLite 数据库连接管理服务
/// </summary>
public class DatabaseService
{
    private SQLiteAsyncConnection? _connection;
    private readonly string _dbPath;
    private bool _initialized = false;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "flavorvault.db3");
    }

    /// <summary>
    /// 初始化数据库，创建所有表
    /// </summary>
    public async Task Init()
    {
        if (_initialized) return;

        await _initLock.WaitAsync();
        try
        {
            if (_initialized) return;

            var conn = GetConnection();
            await conn.CreateTableAsync<FoodEntry>();
            await conn.CreateTableAsync<WishItem>();
            await conn.CreateTableAsync<Collection>();
            await conn.CreateTableAsync<PlaceMark>();
            await conn.CreateTableAsync<UserPreference>();
            await conn.CreateTableAsync<User>();
            _initialized = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DatabaseService] Init error: {ex.Message}");
        }
        finally
        {
            _initLock.Release();
        }
    }

    /// <summary>
    /// 获取 SQLite 异步连接
    /// </summary>
    public SQLiteAsyncConnection GetConnection()
    {
        _connection ??= new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
        return _connection;
    }
}
