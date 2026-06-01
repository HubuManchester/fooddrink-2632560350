using FlavorVault.Services;

namespace FlavorVault.Services;

/// <summary>
/// 首次运行检测服务
/// </summary>
public class FirstRunService
{
    private readonly UserProfileRepository _userProfileRepo;

    public FirstRunService(UserProfileRepository userProfileRepo)
    {
        _userProfileRepo = userProfileRepo;
    }

    /// <summary>
    /// 是否首次运行
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
    /// 标记首次运行已完成
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
