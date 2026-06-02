using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// Catalog list ViewModel, supports search, filter, voice search and view switching
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
    private string _selectedRegion = "All";

    [ObservableProperty]
    private string _selectedRarity = "All";

    [ObservableProperty]
    private string _selectedTaste = "All";

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
    /// Filter result text
    /// </summary>
    public string FilterResultText => $"{FilteredEntries.Count} results total";

    public List<string> Regions { get; } = new()
    {
        "All", "Sichuan", "Cantonese", "Jiangnan", "Northern", "Northwest", "Japan", "Korea", "SoutheastAsia", "Western"
    };

    public List<string> Rarities { get; } = new()
    {
        "All", "Common", "Recommended", "Limited", "Premium"
    };

    public List<string> Tastes { get; } = new()
    {
        "All", "Salty", "Sweet", "Sour", "Spicy", "Umami", "Complex"
    };

    public CatalogViewModel(
        FoodEntryRepository foodEntryRepo,
        SpeechToTextService speechService)
    {
        _foodEntryRepo = foodEntryRepo;
        _speechService = speechService;
        Title = "Catalog";
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
            System.Diagnostics.Debug.WriteLine($"[CatalogViewModel] Load error: {ex.Message}");
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
        SelectedRegion = string.IsNullOrEmpty(region) ? "All" : region;
        ApplyFilter();
    }

    [RelayCommand]
    private void FilterRarity(string? rarity)
    {
        SelectedRarity = string.IsNullOrEmpty(rarity) ? "All" : rarity;
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
            System.Diagnostics.Debug.WriteLine($"[CatalogViewModel] GoToEntryDetail error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[CatalogViewModel] GoToNewEntry error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[CatalogViewModel] VoiceSearch error: {ex.Message}");
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
    /// Filter entries based on current filter criteria
    /// </summary>
    private void ApplyFilter()
    {
        var query = AllEntries.AsEnumerable();

        // Region filter
        if (!string.IsNullOrEmpty(SelectedRegion) && SelectedRegion != "All")
            query = query.Where(e => e.Region == SelectedRegion);

        // Rarity filter
        if (!string.IsNullOrEmpty(SelectedRarity) && SelectedRarity != "All")
            query = query.Where(e => e.Rarity == SelectedRarity);

        // Taste filter
        if (!string.IsNullOrEmpty(SelectedTaste) && SelectedTaste != "All")
            query = query.Where(e => e.PrimaryTaste == SelectedTaste);

        // Search keyword
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
