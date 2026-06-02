using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// Wishlist ViewModel, supports priority filtering, completion marking and CRUD
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
    private string _selectedPriority = "All";

    /// <summary>
    /// Summary statistics text
    /// </summary>
    public string SummaryText => $"{TotalCount} wishes total, {CompletedCount} completed";

    /// <summary>
    /// Progress bar width (for binding, based on screen width 280px ratio)
    /// </summary>
    public double ProgressWidth => TotalCount > 0 ? 280.0 * ProgressPercent / 100.0 : 0;

    /// <summary>
    /// Progress percentage text
    /// </summary>
    public string ProgressPercentText => $"{ProgressPercent:F0}%";

    public List<string> Priorities { get; } = new()
    {
        "All", "Urgent", "WhenFree", "Whenever"
    };

    public WishListViewModel(
        WishItemRepository wishItemRepo,
        HapticService hapticService)
    {
        _wishItemRepo = wishItemRepo;
        _hapticService = hapticService;
        Title = "Wishlist";
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
            System.Diagnostics.Debug.WriteLine($"[WishListViewModel] Load error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[WishListViewModel] FilterByPriority error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[WishListViewModel] ToggleComplete error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddWish()
    {
        try
        {
            // Pop up input dialog
            var name = await Application.Current!.MainPage!.DisplayPromptAsync(
                "Add Wish", "Enter the food name you want to try", "OK", "Cancel",
                "Food name", maxLength: 100);

            if (string.IsNullOrWhiteSpace(name)) return;

            var priority = await Application.Current!.MainPage!.DisplayActionSheet(
                "Select priority", "Cancel", null, "Urgent", "WhenFree", "Whenever");

            if (priority == "Cancel" || string.IsNullOrEmpty(priority)) return;

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
            System.Diagnostics.Debug.WriteLine($"[WishListViewModel] AddWish error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task DeleteWish(WishItem? item)
    {
        if (item is null) return;
        try
        {
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Confirm Delete", $"Are you sure you want to delete \"{item.FoodName}\"?", "Delete", "Cancel");

            if (!confirm) return;

            await _wishItemRepo.DeleteAsync(item);
            WishItems.Remove(item);
            UpdateStats();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WishListViewModel] DeleteWish error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await Load();
    }

    /// <summary>
    /// Load wish items and filter by priority
    /// </summary>
    private async Task LoadWishItems()
    {
        var allItems = await _wishItemRepo.GetAllAsync();

        // Filter by priority
        var filtered = SelectedPriority == "All"
            ? allItems
            : allItems.Where(w => w.Priority == SelectedPriority);

        var sorted = filtered.OrderByDescending(w => w.CreatedAt).ToList();

        WishItems.Clear();
        foreach (var item in sorted)
            WishItems.Add(item);

        // Statistics use all data
        TotalCount = allItems.Count;
        CompletedCount = allItems.Count(w => w.IsCompleted);
        ProgressPercent = TotalCount > 0 ? (double)CompletedCount / TotalCount * 100 : 0;
        OnPropertyChanged(nameof(SummaryText));
        OnPropertyChanged(nameof(ProgressValue));
        OnPropertyChanged(nameof(ProgressPercentText));
    }

    /// <summary>
    /// Update statistics (without reloading the list)
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
            System.Diagnostics.Debug.WriteLine($"[WishListViewModel] UpdateStats error: {ex.Message}");
        }
    }
}
