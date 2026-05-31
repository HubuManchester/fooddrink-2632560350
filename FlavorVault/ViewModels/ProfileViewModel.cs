using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// 个人中心 ViewModel，支持主题/字体切换、数据统计和重置
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
    /// 地区进度列表（XAML 绑定用，RegionProgresses 的别名）
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
        Title = "我的";
    }

    [RelayCommand]
    private async Task Load()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            // 加载用户偏好
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

            // 加载统计数据
            var allEntries = await _foodEntryRepo.GetAllAsync();
            TotalEntries = allEntries.Count;
            CollectedCount = allEntries.Count(e => e.CollectStatus == "已收藏");
            WantToTryCount = allEntries.Count(e => e.CollectStatus == "想尝试");
            ShowcaseCount = allEntries.Count(e => e.IsShowcase);

            var collections = await _collectionRepo.GetAllAsync();
            CollectionCount = collections.Count;

            var placeMarks = await _placeMarkRepo.GetAllAsync();
            PlaceMarkCount = placeMarks.Count;

            // 地区进度
            var calculator = new CatalogCalculator();
            var progresses = calculator.CalculateRegionProgress(allEntries);
            RegionProgresses.Clear();
            foreach (var p in progresses)
                RegionProgresses.Add(p);
            OnPropertyChanged(nameof(RegionProgressList));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] Load 错误: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] SaveUserName 错误: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] SetTheme 错误: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] ChangeTheme 错误: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] ChangeFontSize 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task TestVoiceInput()
    {
        try
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "语音输入测试", "语音输入功能可在录入图鉴页面和搜索页面使用", "确定");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] TestVoiceInput 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task TestTts()
    {
        try
        {
            var tts = new TextToSpeechService();
            await tts.SpeakAsync("味藏朗读测试成功！这是一段测试文字。");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] TestTts 错误: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] TestHaptic 错误: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] GoToNearbyPlaces 错误: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] GoToAbout 错误: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] GoToLogin 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        try
        {
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "退出登录",
                $"确定要退出账号「{LoginUsername}」吗？",
                "退出登录", "取消");

            if (!confirm) return;

            await _authService.LogoutAsync();
            IsLoggedIn = false;
            LoginUsername = string.Empty;
            await Load();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] Logout 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ResetData()
    {
        try
        {
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "重置数据",
                "确定要清除所有数据吗？此操作不可恢复！",
                "确定重置", "取消");

            if (!confirm) return;

            var secondConfirm = await Application.Current!.MainPage!.DisplayAlert(
                "二次确认",
                "所有图鉴条目、心愿清单、收藏集和地点标记都将被删除，是否继续？",
                "确认删除", "放弃");

            if (!secondConfirm) return;

            IsBusy = true;

            // 清除所有数据
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

            // 重新加载
            await Load();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] ResetData 错误: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
