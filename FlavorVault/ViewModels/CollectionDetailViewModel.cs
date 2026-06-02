using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// Collection detail ViewModel, supports sorting, entry browsing and editing
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
    /// Statistics text, format "X items"
    /// </summary>
    public string StatsText => $"{Entries?.Count ?? 0} items";

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
            System.Diagnostics.Debug.WriteLine($"[CollectionDetailViewModel] Load error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[CollectionDetailViewModel] GoToEntryDetail error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[CollectionDetailViewModel] AddEntry error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task EditCollection()
    {
        try
        {
            // Navigate to edit collection (can reuse EntryEdit or a dedicated page)
            await Application.Current!.MainPage!.DisplayAlert(
                "Edit Collection", "Collection editing feature is under development", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionDetailViewModel] EditCollection error: {ex.Message}");
        }
    }

    /// <summary>
    /// Load and sort entries
    /// </summary>
    private async Task LoadEntries()
    {
        try
        {
            var allEntries = await _foodEntryRepo.GetAllAsync();

            // Associate by CollectionName
            var collectionEntries = allEntries
                .Where(e => e.CollectionName == Collection.Name)
                .ToList();

            // Sort
            var sorted = SortMode switch
            {
                "rating" => collectionEntries.OrderByDescending(e => e.StarRating).ToList(),
                "rarity" => collectionEntries.OrderBy(e => GetRarityOrder(e.Rarity)).ToList(),
                _ => collectionEntries.OrderByDescending(e => e.CreatedAt).ToList()
            };

            Entries.Clear();
            foreach (var entry in sorted)
                Entries.Add(entry);

            // Average rating
            AverageRating = collectionEntries.Count > 0
                ? collectionEntries.Average(e => e.StarRating)
                : 0;

            OnPropertyChanged(nameof(StatsText));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CollectionDetailViewModel] LoadEntries error: {ex.Message}");
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
