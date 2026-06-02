using CommunityToolkit.Mvvm.ComponentModel;

namespace FlavorVault.ViewModels;

/// <summary>
/// Base ViewModel providing common busy state and title properties
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    /// <summary>
    /// Sets IsBusy state, executes the action, then restores the state
    /// </summary>
    /// <param name="action">The action to execute</param>
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
