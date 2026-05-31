using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// 图鉴列表 ViewModel，支持搜索、筛选、语音搜索和视图切换
/// </summary>
public partial class CatalogViewModel : BaseViewModel
{
    private readonly FoodEntryRepository _foodEntryRepo;
    private readonly SpeechToTextService _speechService;

    [ObservableProperty]
    private List<FoodEntry> _allEntries = new();

    [ObservableProperty]
    private ObservableCollection<FoodEntry> _filteredEntries = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedRegion = "全部";

    [ObservableProperty]
    private string _selectedRarity = "全部";

    [ObservableProperty]
    private string _selectedTaste = "全部";

    [ObservableProperty]
    private bool _isGridMode = true;

    public bool IsListMode => !IsGridMode;

    [ObservableProperty]
    private bool _isListening;

    [ObservableProperty]
    private bool _isFilterExpanded;

    [ObservableProperty]
    private bool _isRefreshing;

    /// <summary>
    /// 筛选结果文本
    /// </summary>
    public string FilterResultText => $"共 {FilteredEntries.Count} 条结果";

    public List<string> Regions { get; } = new()
    {
        "全部", "川渝", "粤港", "江南", "北方", "西北", "日本", "韩国", "东南亚", "欧美"
    };

    public List<string> Rarities { get; } = new()
    {
        "全部", "日常", "推荐", "限定", "珍藏"
    };

    public List<string> Tastes { get; } = new()
    {
        "全部", "咸", "甜", "酸", "辣", "鲜", "复合"
    };

    public CatalogViewModel(
        FoodEntryRepository foodEntryRepo,
        SpeechToTextService speechService)
    {
        _foodEntryRepo = foodEntryRepo;
        _speechService = speechService;
        Title = "图鉴";
    }

    [RelayCommand]
    private async Task Load()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            AllEntries = await _foodEntryRepo.GetAllAsync();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CatalogViewModel] Load 错误: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private void Search()
    {
        ApplyFilter();
    }

    [RelayCommand]
    private void ToggleFilter()
    {
        IsFilterExpanded = !IsFilterExpanded;
    }

    [RelayCommand]
    private void FilterRegion(string? region)
    {
        SelectedRegion = string.IsNullOrEmpty(region) ? "全部" : region;
        ApplyFilter();
    }

    [RelayCommand]
    private void FilterRarity(string? rarity)
    {
        SelectedRarity = string.IsNullOrEmpty(rarity) ? "全部" : rarity;
        ApplyFilter();
    }

    [RelayCommand]
    private void SwitchToGrid()
    {
        if (!IsGridMode)
        {
            IsGridMode = true;
            OnPropertyChanged(nameof(IsListMode));
        }
    }

    [RelayCommand]
    private void SwitchToList()
    {
        if (IsGridMode)
        {
            IsGridMode = false;
            OnPropertyChanged(nameof(IsListMode));
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
            System.Diagnostics.Debug.WriteLine($"[CatalogViewModel] GoToEntryDetail 错误: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[CatalogViewModel] GoToNewEntry 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task VoiceSearch()
    {
        if (IsListening) return;
        IsListening = true;
        try
        {
            var result = await _speechService.RecognizeAsync();
            if (!string.IsNullOrWhiteSpace(result))
            {
                SearchText = result;
                ApplyFilter();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CatalogViewModel] VoiceSearch 错误: {ex.Message}");
        }
        finally
        {
            IsListening = false;
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await Load();
    }

    /// <summary>
    /// 根据当前筛选条件过滤条目
    /// </summary>
    private void ApplyFilter()
    {
        var query = AllEntries.AsEnumerable();

        // 地区筛选
        if (!string.IsNullOrEmpty(SelectedRegion) && SelectedRegion != "全部")
            query = query.Where(e => e.Region == SelectedRegion);

        // 稀有度筛选
        if (!string.IsNullOrEmpty(SelectedRarity) && SelectedRarity != "全部")
            query = query.Where(e => e.Rarity == SelectedRarity);

        // 口味筛选
        if (!string.IsNullOrEmpty(SelectedTaste) && SelectedTaste != "全部")
            query = query.Where(e => e.PrimaryTaste == SelectedTaste);

        // 搜索关键词
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var keyword = SearchText.Trim().ToLower();
            query = query.Where(e =>
                (e.Name?.ToLower().Contains(keyword) == true) ||
                (e.Description?.ToLower().Contains(keyword) == true) ||
                (e.CatalogNumber?.ToLower().Contains(keyword) == true) ||
                (e.Ingredients?.ToLower().Contains(keyword) == true));
        }

        var results = query.OrderByDescending(e => e.CreatedAt).ToList();
        FilteredEntries.Clear();
        foreach (var entry in results)
            FilteredEntries.Add(entry);
        OnPropertyChanged(nameof(FilterResultText));
    }
}
