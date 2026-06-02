using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// Profile ViewModel, supports theme/font switching, data statistics and reset
/// </summary>
public partial class ProfileViewModel : BaseViewModel
{
    private readonly FoodEntryRepository _foodEntryRepo;
    private readonly WishItemRepository _wishItemRepo;
    private readonly CollectionRepository _collectionRepo;
    private readonly PlaceMarkRepository _placeMarkRepo;
    private readonly UserProfileRepository _userProfileRepo;
    private readonly ThemeService _themeService;
    private readonly HapticService _hapticService;
    private readonly AuthService _authService;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _loginUsername = string.Empty;

    [ObservableProperty]
    private bool _isLoggedIn;

    [ObservableProperty]
    private ThemeMode _theme = ThemeMode.System;

    [ObservableProperty]
    private string _fontSize = "Medium";

    [ObservableProperty]
    private int _totalEntries;

    [ObservableProperty]
    private int _collectedCount;

    [ObservableProperty]
    private int _wantToTryCount;

    [ObservableProperty]
    private int _showcaseCount;

    [ObservableProperty]
    private int _collectionCount;

    [ObservableProperty]
    private int _placeMarkCount;

    [ObservableProperty]
    private ObservableCollection<RegionProgress> _regionProgresses = new();

    /// <summary>
    /// Region progress list (for XAML binding, alias of RegionProgresses)
    /// </summary>
    public ObservableCollection<RegionProgress> RegionProgressList => RegionProgresses;

    public ProfileViewModel(
        FoodEntryRepository foodEntryRepo,
        WishItemRepository wishItemRepo,
        CollectionRepository collectionRepo,
        PlaceMarkRepository placeMarkRepo,
        UserProfileRepository userProfileRepo,
        ThemeService themeService,
        HapticService hapticService,
        AuthService authService)
    {
        _foodEntryRepo = foodEntryRepo;
        _wishItemRepo = wishItemRepo;
        _collectionRepo = collectionRepo;
        _placeMarkRepo = placeMarkRepo;
        _userProfileRepo = userProfileRepo;
        _themeService = themeService;
        _hapticService = hapticService;
        _authService = authService;
        Title = "Profile";
    }

    [RelayCommand]
    private async Task Load()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            // Load user preferences
            UserName = await _userProfileRepo.GetAsync("userName") ?? string.Empty;
            var themeStr = await _userProfileRepo.GetAsync("theme") ?? "System";
            Theme = themeStr switch
            {
                "Light" => ThemeMode.Light,
                "Dark" => ThemeMode.Dark,
                _ => ThemeMode.System
            };
            FontSize = await _userProfileRepo.GetAsync("fontSize") ?? "Medium";

            IsLoggedIn = _authService.IsLoggedIn;
            LoginUsername = _authService.CurrentUser?.DisplayName
                            ?? _authService.CurrentUser?.Username
                            ?? string.Empty;

            // Load statistics
            var allEntries = await _foodEntryRepo.GetAllAsync();
            TotalEntries = allEntries.Count;
            CollectedCount = allEntries.Count(e => e.CollectStatus == "Collected");
            WantToTryCount = allEntries.Count(e => e.CollectStatus == "WantToTry");
            ShowcaseCount = allEntries.Count(e => e.IsShowcase);

            var collections = await _collectionRepo.GetAllAsync();
            CollectionCount = collections.Count;

            var placeMarks = await _placeMarkRepo.GetAllAsync();
            PlaceMarkCount = placeMarks.Count;

            // Region progress
            var calculator = new CatalogCalculator();
            var progresses = calculator.CalculateRegionProgress(allEntries);
            RegionProgresses.Clear();
            foreach (var p in progresses)
                RegionProgresses.Add(p);
            OnPropertyChanged(nameof(RegionProgressList));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] Load error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveUserName()
    {
        try
        {
            await _userProfileRepo.SetAsync("userName", UserName);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] SaveUserName error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SetTheme(string themeStr)
    {
        try
        {
            if (Enum.TryParse<ThemeMode>(themeStr, out var theme))
            {
                Theme = theme;
                await _themeService.SetThemeAsync(theme);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] SetTheme error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ChangeTheme(ThemeMode theme)
    {
        try
        {
            Theme = theme;
            await _themeService.SetThemeAsync(theme);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] ChangeTheme error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SetFontSize(string fontSize)
    {
        try
        {
            FontSize = fontSize;
            if (Enum.TryParse<FontSizeOption>(fontSize, out var size))
            {
                await _themeService.SetFontSizeAsync(size);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] ChangeFontSize error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task TestVoiceInput()
    {
        try
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Voice Input Test", "Voice input is available on the entry and search pages", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] TestVoiceInput error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task TestTts()
    {
        try
        {
            var tts = new TextToSpeechService();
            await tts.SpeakAsync("FoodDrinkApp TTS test successful! This is a test sentence.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] TestTts error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task TestHaptic()
    {
        try
        {
            await _hapticService.SuccessAsync();
            await Task.Delay(300);
            await _hapticService.ErrorAsync();
            await Task.Delay(300);
            await _hapticService.LightAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] TestHaptic error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToNearbyPlaces()
    {
        try
        {
            await Shell.Current.GoToAsync("NearbyPlacesPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] GoToNearbyPlaces error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToAbout()
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(Views.AboutPage));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] GoToAbout error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToLogin()
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(Views.LoginPage));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] GoToLogin error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        try
        {
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Sign Out",
                $"Are you sure you want to sign out of \"{LoginUsername}\"?",
                "Sign Out", "Cancel");

            if (!confirm) return;

            await _authService.LogoutAsync();
            IsLoggedIn = false;
            LoginUsername = string.Empty;
            await Load();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] Logout error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ResetData()
    {
        try
        {
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Reset Data",
                "Are you sure you want to clear all data? This cannot be undone!",
                "Confirm Reset", "Cancel");

            if (!confirm) return;

            var secondConfirm = await Application.Current!.MainPage!.DisplayAlert(
                "Confirm Again",
                "All entries, wishlists, collections, and place marks will be deleted. Continue?",
                "Confirm Delete", "Discard");

            if (!secondConfirm) return;

            IsBusy = true;

            // Clear all data
            var entries = await _foodEntryRepo.GetAllAsync();
            foreach (var entry in entries)
                await _foodEntryRepo.DeleteAsync(entry);

            var wishes = await _wishItemRepo.GetAllAsync();
            foreach (var wish in wishes)
                await _wishItemRepo.DeleteAsync(wish);

            var collections = await _collectionRepo.GetAllAsync();
            foreach (var collection in collections)
                await _collectionRepo.DeleteAsync(collection);

            var placeMarks = await _placeMarkRepo.GetAllAsync();
            foreach (var mark in placeMarks)
                await _placeMarkRepo.DeleteAsync(mark);

            // Reload
            await Load();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] ResetData error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
