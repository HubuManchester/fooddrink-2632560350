using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// 地区图鉴详情 ViewModel，展示某地区的所有条目和统计
/// </summary>
public partial class RegionDetailViewModel : BaseViewModel
{
    private readonly FoodEntryRepository _foodEntryRepo;
    private readonly CatalogCalculator _calculator;

    [ObservableProperty]
    private string _regionName = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private ObservableCollection<FoodEntry> _entries = new();

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _commonCount;

    [ObservableProperty]
    private int _uncommonCount;

    [ObservableProperty]
    private int _rareCount;

    [ObservableProperty]
    private int _epicCount;

    /// <summary>
    /// 地区描述文本（XAML 绑定用，Description 的别名）
    /// </summary>
    public string RegionDescription => Description;

    /// <summary>
    /// 进度百分比（0.0 ~ 1.0，用于 ProgressBar.Progress 绑定）
    /// </summary>
    public double ProgressPercent => TotalCount > 0 ? 1.0 : 0.0;

    /// <summary>
    /// 进度文本
    /// </summary>
    public string ProgressText => $"已收录 {TotalCount} 件";

    public RegionDetailViewModel(
        FoodEntryRepository foodEntryRepo,
        CatalogCalculator calculator)
    {
        _foodEntryRepo = foodEntryRepo;
        _calculator = calculator;
    }

    [RelayCommand]
    private async Task Load(string region)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            RegionName = region;
            Title = region + "图鉴";

            var allEntries = await _foodEntryRepo.GetAllAsync();
            var regionEntries = allEntries
                .Where(e => e.Region == region)
                .ToList();

            // 按稀有度排序（珍藏 > 限定 > 推荐 > 日常）
            var sorted = regionEntries
                .OrderBy(e => GetRarityOrder(e.Rarity))
                .ToList();

            Entries.Clear();
            foreach (var entry in sorted)
                Entries.Add(entry);

            // 统计
            TotalCount = regionEntries.Count;
            CommonCount = regionEntries.Count(e => e.Rarity == "日常");
            UncommonCount = regionEntries.Count(e => e.Rarity == "推荐");
            RareCount = regionEntries.Count(e => e.Rarity == "限定");
            EpicCount = regionEntries.Count(e => e.Rarity == "珍藏");

            // 描述
            Description = $"共 {TotalCount} 件藏品，" +
                         $"日常 {CommonCount} / 推荐 {UncommonCount} / 限定 {RareCount} / 珍藏 {EpicCount}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[RegionDetailViewModel] Load 错误: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
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
            System.Diagnostics.Debug.WriteLine($"[RegionDetailViewModel] GoToEntryDetail 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoToNewEntry()
    {
        try
        {
            await Shell.Current.GoToAsync($"EntryEditPage?region={Uri.EscapeDataString(RegionName)}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[RegionDetailViewModel] GoToNewEntry 错误: {ex.Message}");
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
