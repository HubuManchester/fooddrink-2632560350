using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

public partial class ShowcaseManageViewModel : BaseViewModel
{
    private readonly FoodEntryRepository _foodEntryRepo;

    public class ShowcaseItem
    {
        public FoodEntry Entry { get; set; } = null!;
        public bool IsShowcase { get; set; }
        public string DisplayText => IsShowcase ? "✓ Showcased" : "Not Showcased";
    }

    [ObservableProperty]
    private ObservableCollection<ShowcaseItem> _items = new();

    [ObservableProperty]
    private string _summaryText = string.Empty;

    public ShowcaseManageViewModel(FoodEntryRepository foodEntryRepo)
    {
        _foodEntryRepo = foodEntryRepo;
        Title = "Manage Showcase";
    }

    [RelayCommand]
    private async Task Load()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var all = await _foodEntryRepo.GetAllAsync();
            Items.Clear();
            foreach (var entry in all.OrderByDescending(e => e.IsShowcase).ThenByDescending(e => e.StarRating))
            {
                Items.Add(new ShowcaseItem { Entry = entry, IsShowcase = entry.IsShowcase });
            }
            UpdateSummary();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShowcaseManageVM] Load: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ToggleShowcase(ShowcaseItem? item)
    {
        if (item == null) return;

        var showcaseCount = Items.Count(i => i.IsShowcase);
        if (!item.IsShowcase && showcaseCount >= 5)
        {
            await Shell.Current.DisplayAlert("Notice", "Showcase can display at most 5 dishes, please remove others first", "OK");
            return;
        }

        item.IsShowcase = !item.IsShowcase;
        item.Entry.IsShowcase = item.IsShowcase;
        await _foodEntryRepo.UpdateAsync(item.Entry);
        UpdateSummary();

        var idx = Items.IndexOf(item);
        Items[idx] = item;
    }

    private void UpdateSummary()
    {
        var count = Items.Count(i => i.IsShowcase);
        SummaryText = $"{count}/5 showcase dishes selected";
    }
}
