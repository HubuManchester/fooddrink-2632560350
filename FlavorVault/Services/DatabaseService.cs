using SQLite;
using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// SQLite database connection management service
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
    /// Initialize the database and create all tables
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
    /// Get the SQLite async connection
    /// </summary>
    public SQLiteAsyncConnection GetConnection()
    {
        _connection ??= new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
        return _connection;
    }
}
