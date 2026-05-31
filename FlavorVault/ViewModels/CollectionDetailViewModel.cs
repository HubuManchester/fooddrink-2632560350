using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// 收藏集详情 ViewModel，支持排序、条目浏览和编辑
/// </summary>
public partial class CollectionDetailViewModel : BaseViewModel
{
    private readonly CollectionRepository _collectionRepo;
    private readonly FoodEntryRepository _foodEntryRepo;

    [ObservableProperty]
    private Collection _collection = new();

    [ObservableProperty]
    private ObservableCollection<FoodEntry> _entries = new();

    [ObservableProperty]
    private double _averageRating;

    [ObservableProperty]
    private string _sortMode = "recent";

    public CollectionDetailViewModel(
        CollectionRepository collectionRepo,
        FoodEntryRepository foodEntryRepo)
    {
        _collectionRepo = collectionRepo;
        _foodEntryRepo = foodEntryRepo;
    }

    /// <summary>
    /// 统计文本，格式 "X 件藏品"
    /// </summary>
    public string StatsText => $"{Entries?.Count ?? 0} 件藏品";

    [RelayCommand]
    private async Task Load(int collectionId)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var collection = await _collectionRepo.GetByIdAsync(collectionId);
            if (collection is not null)
            {
                Collection = collection;
                Title = collection.Name;
            }

            await LoadEntries();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionDetailViewModel] Load 错误: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SortByRating()
    {
        SortMode = "rating";
        await LoadEntries();
    }

    [RelayCommand]
    private async Task SortByRarity()
    {
        SortMode = "rarity";
        await LoadEntries();
    }

    [RelayCommand]
    private async Task SortByRecent()
    {
        SortMode = "recent";
        await LoadEntries();
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
            System.Diagnostics.Debug.WriteLine($"[CollectionDetailViewModel] GoToEntryDetail 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddEntry()
    {
        try
        {
            await Shell.Current.GoToAsync("EntryEditPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionDetailViewModel] AddEntry 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task EditCollection()
    {
        try
        {
            // 导航到编辑收藏集（可复用 EntryEdit 或专用页面）
            await Application.Current!.MainPage!.DisplayAlert(
                "编辑收藏集", "收藏集编辑功能开发中", "确定");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionDetailViewModel] EditCollection 错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 加载并排序条目
    /// </summary>
    private async Task LoadEntries()
    {
        try
        {
            var allEntries = await _foodEntryRepo.GetAllAsync();

            // 通过 CollectionName 关联
            var collectionEntries = allEntries
                .Where(e => e.CollectionName == Collection.Name)
                .ToList();

            // 排序
            var sorted = SortMode switch
            {
                "rating" => collectionEntries.OrderByDescending(e => e.StarRating).ToList(),
                "rarity" => collectionEntries.OrderBy(e => GetRarityOrder(e.Rarity)).ToList(),
                _ => collectionEntries.OrderByDescending(e => e.CreatedAt).ToList()
            };

            Entries.Clear();
            foreach (var entry in sorted)
                Entries.Add(entry);

            // 平均星级
            AverageRating = collectionEntries.Count > 0
                ? collectionEntries.Average(e => e.StarRating)
                : 0;

            OnPropertyChanged(nameof(StatsText));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionDetailViewModel] LoadEntries 错误: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取稀有度排序权重（珍藏 > 限定 > 推荐 > 日常）
    /// </summary>
    private static int GetRarityOrder(string? rarity)
    {
        return rarity switch
        {
            "珍藏" => 0,
            "限定" => 1,
            "推荐" => 2,
            "日常" => 3,
            _ => 4
        };
    }
}
