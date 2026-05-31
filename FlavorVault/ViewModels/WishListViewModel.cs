using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// 心愿清单 ViewModel，支持优先级筛选、完成标记和 CRUD
/// </summary>
public partial class WishListViewModel : BaseViewModel
{
    private readonly WishItemRepository _wishItemRepo;
    private readonly HapticService _hapticService;

    [ObservableProperty]
    private ObservableCollection<WishItem> _wishItems = new();

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _completedCount;

    [ObservableProperty]
    private double _progressPercent;

    public double ProgressValue => ProgressPercent / 100.0;

    [ObservableProperty]
    private string _selectedPriority = "全部";

    /// <summary>
    /// 统计摘要文本
    /// </summary>
    public string SummaryText => $"共 {TotalCount} 个心愿，已完成 {CompletedCount} 个";

    /// <summary>
    /// 进度条宽度（绑定用，基于屏幕宽度 280px 比例）
    /// </summary>
    public double ProgressWidth => TotalCount > 0 ? 280.0 * ProgressPercent / 100.0 : 0;

    /// <summary>
    /// 进度百分比文本
    /// </summary>
    public string ProgressPercentText => $"{ProgressPercent:F0}%";

    public List<string> Priorities { get; } = new()
    {
        "全部", "尽快", "有空就去", "随缘"
    };

    public WishListViewModel(
        WishItemRepository wishItemRepo,
        HapticService hapticService)
    {
        _wishItemRepo = wishItemRepo;
        _hapticService = hapticService;
        Title = "心愿清单";
    }

    [RelayCommand]
    private async Task Load()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await LoadWishItems();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishListViewModel] Load 错误: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task FilterByPriority(string? priority)
    {
        if (!string.IsNullOrEmpty(priority))
            SelectedPriority = priority;
        try
        {
            await LoadWishItems();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishListViewModel] FilterByPriority 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ToggleComplete(WishItem? item)
    {
        if (item is null) return;
        try
        {
            item.IsCompleted = !item.IsCompleted;
            await _wishItemRepo.UpdateAsync(item);

            await _hapticService.SuccessAsync();

            var index = WishItems.IndexOf(item);
            if (index >= 0)
            {
                WishItems[index] = item;
            }

            UpdateStats();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishListViewModel] ToggleComplete 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddWish()
    {
        try
        {
            // 弹出输入对话框
            var name = await Application.Current!.MainPage!.DisplayPromptAsync(
                "添加心愿", "输入想尝试的食物名称", "确定", "取消",
                "食物名称", maxLength: 100);

            if (string.IsNullOrWhiteSpace(name)) return;

            var priority = await Application.Current!.MainPage!.DisplayActionSheet(
                "选择优先级", "取消", null, "尽快", "有空就去", "随缘");

            if (priority == "取消" || string.IsNullOrEmpty(priority)) return;

            var item = new WishItem
            {
                FoodName = name.Trim(),
                Priority = priority,
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await _wishItemRepo.InsertAsync(item);
            await LoadWishItems();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishListViewModel] AddWish 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task DeleteWish(WishItem? item)
    {
        if (item is null) return;
        try
        {
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "确认删除", $"确定要删除「{item.FoodName}」吗？", "删除", "取消");

            if (!confirm) return;

            await _wishItemRepo.DeleteAsync(item);
            WishItems.Remove(item);
            UpdateStats();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishListViewModel] DeleteWish 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await Load();
    }

    /// <summary>
    /// 加载心愿条目并根据优先级筛选
    /// </summary>
    private async Task LoadWishItems()
    {
        var allItems = await _wishItemRepo.GetAllAsync();

        // 筛选优先级
        var filtered = SelectedPriority == "全部"
            ? allItems
            : allItems.Where(w => w.Priority == SelectedPriority);

        var sorted = filtered.OrderByDescending(w => w.CreatedAt).ToList();

        WishItems.Clear();
        foreach (var item in sorted)
            WishItems.Add(item);

        // 统计使用全部数据
        TotalCount = allItems.Count;
        CompletedCount = allItems.Count(w => w.IsCompleted);
        ProgressPercent = TotalCount > 0 ? (double)CompletedCount / TotalCount * 100 : 0;
        OnPropertyChanged(nameof(SummaryText));
        OnPropertyChanged(nameof(ProgressValue));
        OnPropertyChanged(nameof(ProgressPercentText));
    }

    /// <summary>
    /// 更新统计数据（不重新加载列表）
    /// </summary>
    private async Task UpdateStats()
    {
        try
        {
            var allItems = await _wishItemRepo.GetAllAsync();
            TotalCount = allItems.Count;
            CompletedCount = allItems.Count(w => w.IsCompleted);
            ProgressPercent = TotalCount > 0 ? (double)CompletedCount / TotalCount * 100 : 0;
            OnPropertyChanged(nameof(SummaryText));
            OnPropertyChanged(nameof(ProgressValue));
            OnPropertyChanged(nameof(ProgressPercentText));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishListViewModel] UpdateStats 错误: {ex.Message}");
        }
    }
}
