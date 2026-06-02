using System.Security.Cryptography;
using System.Text;
using FlavorVault.Models;

namespace FlavorVault.Services;

public class AuthService
{
    private readonly DatabaseService _dbService;
    private User? _currentUser;

    public event Action<bool>? LoginStateChanged;

    public User? CurrentUser => _currentUser;

    public bool IsLoggedIn => _currentUser is not null;

    public AuthService(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    public async Task InitAsync()
    {
        await _dbService.Init();
    }

    public async Task<(bool Success, string Message)> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            return (false, "Please enter username");
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Please enter password");

        await InitAsync();
        var conn = _dbService.GetConnection();

        var hash = HashPassword(password);
        var user = await conn.Table<User>()
            .Where(u => u.Username == username && u.PasswordHash == hash)
            .FirstOrDefaultAsync();

        if (user is null)
            return (false, "Invalid username or password");

        _currentUser = user;
        user.LastLoginAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        await conn.UpdateAsync(user);

        await SaveSessionAsync(user.Id);

        LoginStateChanged?.Invoke(true);
        return (true, "Login successful");
    }

    public async Task<(bool Success, string Message)> RegisterAsync(string username, string password, string displayName)
    {
        if (string.IsNullOrWhiteSpace(username))
            return (false, "Please enter username");
        if (username.Length < 3)
            return (false, "Username must be at least 3 characters");
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Please enter password");
        if (password.Length < 6)
            return (false, "Password must be at least 6 characters");

        await InitAsync();
        var conn = _dbService.GetConnection();

        var existing = await conn.Table<User>()
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();

        if (existing is not null)
            return (false, "Username already taken");

        var user = new User
        {
            Username = username,
            PasswordHash = HashPassword(password),
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? username : displayName,
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        await conn.InsertAsync(user);

        _currentUser = user;
        await SaveSessionAsync(user.Id);

        LoginStateChanged?.Invoke(true);
        return (true, "Registration successful");
    }

    public async Task LogoutAsync()
    {
        _currentUser = null;
        await ClearSessionAsync();
        LoginStateChanged?.Invoke(false);
    }

    public async Task<bool> TryRestoreSessionAsync()
    {
        try
        {
            await InitAsync();
            var userId = await GetSavedUserIdAsync();
            if (userId <= 0) return false;

            var conn = _dbService.GetConnection();
            var user = await conn.Table<User>().Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user is null) return false;

            _currentUser = user;
            LoginStateChanged?.Invoke(true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateProfileAsync(string displayName)
    {
        if (_currentUser is null) return false;
        try
        {
            _currentUser.DisplayName = displayName;
            await InitAsync();
            await _dbService.GetConnection().UpdateAsync(_currentUser);
            return true;
        }
        catch { return false; }
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    private async Task SaveSessionAsync(int userId)
    {
        try
        {
            await InitAsync();
            var conn = _dbService.GetConnection();
            await conn.InsertOrReplaceAsync(new UserPreference { Key = "logged_in_user_id", Value = userId.ToString() });
        }
        catch { }
    }

    private async Task ClearSessionAsync()
    {
        try
        {
            await InitAsync();
            var conn = _dbService.GetConnection();
            var pref = await conn.Table<UserPreference>()
                .Where(p => p.Key == "logged_in_user_id")
                .FirstOrDefaultAsync();
            if (pref is not null)
                await conn.DeleteAsync(pref);
        }
        catch { }
    }

    private async Task<int> GetSavedUserIdAsync()
    {
        try
        {
            await InitAsync();
            var conn = _dbService.GetConnection();
            var pref = await conn.Table<UserPreference>()
                .Where(p => p.Key == "logged_in_user_id")
                .FirstOrDefaultAsync();
            if (pref is not null && int.TryParse(pref.Value, out var id))
                return id;
        }
        catch { }
        return -1;
    }
}
