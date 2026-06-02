using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// 首页 ViewModel，展示收藏概览、进度、稀有度统计和橱窗精选
/// </summary>
public partial class HomeViewModel : BaseViewModel
{
    private readonly FoodEntryRepository _foodEntryRepo;
    private readonly CollectionRepository _collectionRepo;
    private readonly PlaceMarkRepository _placeMarkRepo;
    private readonly CatalogCalculator _calculator;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private ObservableCollection<RegionProgress> _regionProgresses = new();

    [ObservableProperty]
    private ObservableCollection<RarityCount> _rarityStats = new();

    [ObservableProperty]
    private ObservableCollection<FoodEntry> _showcaseEntries = new();

    [ObservableProperty]
    private FoodEntry? _shakeResult;

    [ObservableProperty]
    private bool _hasShakeResult;

    /// <summary>
    /// 是否正在刷新（XAML 绑定用）
    /// </summary>
    [ObservableProperty]
    private bool _isRefreshing;

    /// <summary>
    /// 条目数量文本（XAML 绑定用）
    /// </summary>
    public string EntryCountText => $"已收录 {TotalCount} 道美食";

    public HomeViewModel(
        FoodEntryRepository foodEntryRepo,
        CollectionRepository collectionRepo,
        PlaceMarkRepository placeMarkRepo,
        CatalogCalculator calculator)
    {
        _foodEntryRepo = foodEntryRepo;
        _collectionRepo = collectionRepo;
        _placeMarkRepo = placeMarkRepo;
        _calculator = calculator;
        Title = "我的图鉴";
    }

    [RelayCommand]
    private async Task LoadData()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var allEntries = await _foodEntryRepo.GetAllAsync();

            // 总数
            TotalCount = allEntries.Count;
            OnPropertyChanged(nameof(EntryCountText));

            // 地区进度
            var progresses = _calculator.CalculateRegionProgress(allEntries);
            RegionProgresses.Clear();
            foreach (var p in progresses)
                RegionProgresses.Add(p);

            // 稀有度统计
            var rarities = _calculator.CalculateRarityStats(allEntries);
            RarityStats.Clear();
            foreach (var r in rarities)
                RarityStats.Add(r);

            // 橱窗精选（IsShowcase=true，最多5个）
            var showcases = allEntries
                .Where(e => e.IsShowcase)
                .Take(5)
                .ToList();
            ShowcaseEntries.Clear();
            foreach (var entry in showcases)
                ShowcaseEntries.Add(entry);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] LoadData 错误: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadData();
    }

    [RelayCommand]
    private async Task GoToCamera()
    {
        try
        {
            await Shell.Current.GoToAsync("//CameraPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToCamera 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToCatalog()
    {
        try
        {
            await Shell.Current.GoToAsync("//CatalogPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToCatalog 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToExplore()
    {
        try
        {
            await Shell.Current.GoToAsync("//ExplorePage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToExplore 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToNewEntry()
    {
        try
        {
            await Shell.Current.GoToAsync("EntryEditPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToNewEntry 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToEntryDetail(FoodEntry? entry)
    {
        if (entry is null) return;
        try
        {
            await Shell.Current.GoToAsync($"EntryDetailPage?id={entry.Id}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] GoToEntryDetail 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ShakeDiscover()
    {
        try
        {
            var allEntries = await _foodEntryRepo.GetAllAsync();
            if (allEntries.Count == 0) return;

            var random = new Random();
            var index = random.Next(allEntries.Count);
            ShakeResult = allEntries[index];
            HasShakeResult = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] ShakeDiscover 错误: {ex.Message}");
        }
    }
}
