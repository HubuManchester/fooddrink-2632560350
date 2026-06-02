using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// Region catalog detail ViewModel, displays all entries and statistics for a region
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
    private int _recommendedCount;

    [ObservableProperty]
    private int _limitedCount;

    [ObservableProperty]
    private int _premiumCount;

    /// <summary>
    /// Region description text (for XAML binding, alias of Description)
    /// </summary>
    public string RegionDescription => Description;

    /// <summary>
    /// Progress percentage (0.0 ~ 1.0, for ProgressBar.Progress binding)
    /// </summary>
    public double ProgressPercent => TotalCount > 0 ? 1.0 : 0.0;

    /// <summary>
    /// Progress text
    /// </summary>
    public string ProgressText => $"{TotalCount} items collected";

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
            Title = region + " Catalog";

            var allEntries = await _foodEntryRepo.GetAllAsync();
            var regionEntries = allEntries
                .Where(e => e.Region == region)
                .ToList();

            // Sort by rarity (Premium > Limited > Recommended > Common)
            var sorted = regionEntries
                .OrderBy(e => GetRarityOrder(e.Rarity))
                .ToList();

            Entries.Clear();
            foreach (var entry in sorted)
                Entries.Add(entry);

            // Statistics
            TotalCount = regionEntries.Count;
            CommonCount = regionEntries.Count(e => e.Rarity == "Common");
            RecommendedCount = regionEntries.Count(e => e.Rarity == "Recommended");
            LimitedCount = regionEntries.Count(e => e.Rarity == "Limited");
            PremiumCount = regionEntries.Count(e => e.Rarity == "Premium");

            // Description
            Description = $"Total {TotalCount} items, " +
                         $"Common {CommonCount} / Recommended {RecommendedCount} / Limited {LimitedCount} / Premium {PremiumCount}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[RegionDetailViewModel] Load error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[RegionDetailViewModel] GoToEntryDetail error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[RegionDetailViewModel] GoToNewEntry error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get rarity sort weight (Premium > Limited > Recommended > Common)
    /// </summary>
    private static int GetRarityOrder(string? rarity)
    {
        return rarity switch
        {
            "Premium" => 0,
            "Limited" => 1,
            "Recommended" => 2,
            "Common" => 3,
            _ => 4
        };
    }
}
