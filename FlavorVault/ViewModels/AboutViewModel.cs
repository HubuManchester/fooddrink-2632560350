using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// 关于页面 ViewModel
/// </summary>
public partial class AboutViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _appVersion = "1.0.0";

    [ObservableProperty]
    private string _appName = "味藏 FlavorVault";

    [ObservableProperty]
    private string _description = "你的专属食物收藏图鉴，像编撰百科全书一样记录每一道美食。";

    public AboutViewModel()
    {
        Title = "关于";
        try
        {
            AppVersion = VersionTracking.CurrentVersion ?? "1.0.0";
        }
        catch { }
    }

    /// <summary>
    /// 版本显示文本（XAML 别名绑定）
    /// </summary>
    public string VersionText => $"版本 {AppVersion}";

    [RelayCommand]
    private async Task Load()
    {
        // 无需加载额外数据
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task GoBack()
    {
        try
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
        catch { }
    }
}
