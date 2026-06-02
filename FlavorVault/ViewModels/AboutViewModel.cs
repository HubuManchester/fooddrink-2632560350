using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// About page ViewModel
/// </summary>
public partial class AboutViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _appVersion = "1.0.0";

    [ObservableProperty]
    private string _appName = "FoodDrinkApp FlavorVault";

    [ObservableProperty]
    private string _description = "Your personal food collection catalog, documenting every dish like compiling an encyclopedia.";

    public AboutViewModel()
    {
        Title = "About";
        try
        {
            AppVersion = VersionTracking.CurrentVersion ?? "1.0.0";
        }
        catch { }
    }

    /// <summary>
    /// Version display text (XAML alias binding)
    /// </summary>
    public string VersionText => $"Version {AppVersion}";

    [RelayCommand]
    private async Task Load()
    {
        // No extra data to load
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
