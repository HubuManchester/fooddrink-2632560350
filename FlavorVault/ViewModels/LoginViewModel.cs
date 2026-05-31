using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly AuthService _authService;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isPasswordObscured = true;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
        Title = "登录";
    }

    [RelayCommand]
    private async Task Login()
    {
        HasError = false;
        ErrorMessage = string.Empty;

        var (success, message) = await _authService.LoginAsync(Username, Password);
        if (success)
        {
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            HasError = true;
            ErrorMessage = message;
        }
    }

    [RelayCommand]
    private async Task GoToRegister()
    {
        await Shell.Current.GoToAsync("RegisterPage");
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordObscured = !IsPasswordObscured;
    }
}
