using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.ViewModels;

/// <summary>
/// ViewModel 基类，提供公共的忙碌状态和标题属性
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    /// <summary>
    /// 设置 IsBusy 状态后执行操作，操作完成后恢复
    /// </summary>
    /// <param name="action">要执行的操作</param>
    public void SetBusy(Action action)
    {
        IsBusy = true;
        try
        {
            action();
        }
        finally
        {
            IsBusy = false;
        }
    }
}
