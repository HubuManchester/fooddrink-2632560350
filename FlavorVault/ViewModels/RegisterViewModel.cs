using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly AuthService _authService;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isPasswordObscured = true;

    public RegisterViewModel(AuthService authService)
    {
        _authService = authService;
        Title = "注册";
    }

    [RelayCommand]
    private async Task Register()
    {
        HasError = false;
        ErrorMessage = string.Empty;

        if (Password != ConfirmPassword)
        {
            HasError = true;
            ErrorMessage = "两次输入的密码不一致";
            return;
        }

        var (success, message) = await _authService.RegisterAsync(Username, Password, DisplayName);
        if (success)
        {
            await Shell.Current.GoToAsync("//home");
        }
        else
        {
            HasError = true;
            ErrorMessage = message;
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordObscured = !IsPasswordObscured;
    }
}
