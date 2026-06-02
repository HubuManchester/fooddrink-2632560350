using FlavorVault.Services;

namespace FlavorVault.Services;

/// <summary>
/// First run detection service
/// </summary>
public class FirstRunService
{
    private readonly UserProfileRepository _userProfileRepo;

    public FirstRunService(UserProfileRepository userProfileRepo)
    {
        _userProfileRepo = userProfileRepo;
    }

    /// <summary>
    /// Check if this is the first run
    /// </summary>
    public async Task<bool> IsFirstRunAsync()
    {
        try
        {
            return await _userProfileRepo.IsFirstRunAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FirstRunService] IsFirstRunAsync error: {ex.Message}");
            return true;
        }
    }

    /// <summary>
    /// Mark first run as completed
    /// </summary>
    public async Task MarkCompletedAsync()
    {
        try
        {
            await _userProfileRepo.MarkFirstRunCompletedAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FirstRunService] MarkCompletedAsync error: {ex.Message}");
        }
    }
}
