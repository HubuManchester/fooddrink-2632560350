using System.Collections.ObjectModel;
using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// Entry edit/create ViewModel, supports photo capture, voice input, geolocation and more
/// </summary>
public partial class EntryEditViewModel : BaseViewModel
{
    private readonly FoodEntryRepository _foodEntryRepo;
    private readonly CollectionRepository _collectionRepo;
    private readonly CameraService _cameraService;
    private readonly SpeechToTextService _speechService;
    private readonly GeolocationService _geolocationService;
    private readonly HapticService _hapticService;

    [ObservableProperty]
    private FoodEntry _entry = new();

    [ObservableProperty]
    private bool _isNew = true;

    [ObservableProperty]
    private string _imagePath = string.Empty;

    [ObservableProperty]
    private int _selectedRegionIndex;

    [ObservableProperty]
    private int _selectedRarityIndex;

    [ObservableProperty]
    private int _selectedPriceIndex;

    [ObservableProperty]
    private int _selectedTasteIndex;

    [ObservableProperty]
    private int _selectedAromaIndex;

    [ObservableProperty]
    private int _selectedTextureIndex;

    [ObservableProperty]
    private int _starRating = 3;

    [ObservableProperty]
    private string _ingredientsText = string.Empty;

    [ObservableProperty]
    private string _noteText = string.Empty;

    [ObservableProperty]
    private string _locationName = string.Empty;

    [ObservableProperty]
    private DateTime? _discoverDate;

    [ObservableProperty]
    private ObservableCollection<CollectionCheckItem> _collections = new();

    [ObservableProperty]
    private HashSet<string> _selectedCollections = new();

    [ObservableProperty]
    private bool _isListening;

    [ObservableProperty]
    private bool _isFlashOn;

    [ObservableProperty]
    private bool _isLocating;

    /// <summary>
    /// Whether an image exists (for XAML binding)
    /// </summary>
    public bool HasImage => !string.IsNullOrWhiteSpace(ImagePath);

    /// <summary>
    /// Food name (for XAML binding, wraps Entry.Name)
    /// </summary>
    public string Name
    {
        get => Entry.Name;
        set { Entry.Name = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Whether star 1 is lit
    /// </summary>
    public bool IsStar1 => StarRating >= 1;
    public bool IsStar2 => StarRating >= 2;
    public bool IsStar3 => StarRating >= 3;
    public bool IsStar4 => StarRating >= 4;
    public bool IsStar5 => StarRating >= 5;

    public List<string> Regions { get; } = new()
    {
        "Sichuan", "Cantonese", "Jiangnan", "Northern", "Northwest", "Japan", "Korea", "SoutheastAsia", "Western"
    };

    public List<string> Rarities { get; } = new()
    {
        "Common", "Recommended", "Limited", "Premium"
    };

    public List<string> PriceRanges { get; } = new()
    {
        "~10", "10~30", "30~60", "60~100", "100+"
    };

    public List<string> Tastes { get; } = new()
    {
        "Salty", "Sweet", "Sour", "Spicy", "Umami", "Complex"
    };

    public List<string> Aromas { get; } = new()
    {
        "Garlic", "SoySauce", "Floral", "Smoky", "Creamy", "Herbal", "Fruity", "None"
    };

    public List<string> Textures { get; } = new()
    {
        "Crispy", "Tender", "Silky", "Chewy", "Refreshing", "Rich", "Firm"
    };

    public EntryEditViewModel(
        FoodEntryRepository foodEntryRepo,
        CollectionRepository collectionRepo,
        CameraService cameraService,
        SpeechToTextService speechService,
        GeolocationService geolocationService,
        HapticService hapticService)
    {
        _foodEntryRepo = foodEntryRepo;
        _collectionRepo = collectionRepo;
        _cameraService = cameraService;
        _speechService = speechService;
        _geolocationService = geolocationService;
        _hapticService = hapticService;
        Title = "New Entry";
    }

    [RelayCommand]
    private async Task LoadEntry(int? id)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            FoodEntry? existingEntry = null;
            if (id.HasValue && id.Value > 0)
            {
                existingEntry = await _foodEntryRepo.GetByIdAsync(id.Value);
            }

            var allCollections = await _collectionRepo.GetAllAsync();
            Collections.Clear();
            foreach (var c in allCollections)
            {
                var item = new CollectionCheckItem { Name = c.Name };
                if (!string.IsNullOrWhiteSpace(existingEntry?.CollectionName) && c.Name == existingEntry.CollectionName)
                    item.IsChecked = true;
                Collections.Add(item);
            }

            if (existingEntry is not null)
            {
                // Edit mode
                var entry = existingEntry;
                if (entry is not null)
                {
                    Entry = entry;
                    IsNew = false;
                    Title = "Edit Entry";

                    // Restore Picker selected indices
                    ImagePath = entry.ImagePath ?? string.Empty;
                    OnPropertyChanged(nameof(HasImage));
                    Name = entry.Name;

                    SelectedRegionIndex = Regions.IndexOf(entry.Region) is >= 0 ? Regions.IndexOf(entry.Region) : 0;
                    SelectedRarityIndex = Rarities.IndexOf(entry.Rarity) is >= 0 ? Rarities.IndexOf(entry.Rarity) : 0;
                    SelectedPriceIndex = PriceRanges.IndexOf(entry.PriceRange) is >= 0 ? PriceRanges.IndexOf(entry.PriceRange) : 1;
                    SelectedTasteIndex = !string.IsNullOrEmpty(entry.PrimaryTaste) && Tastes.IndexOf(entry.PrimaryTaste) is >= 0
                        ? Tastes.IndexOf(entry.PrimaryTaste) : -1;
                    SelectedAromaIndex = !string.IsNullOrEmpty(entry.AromaTag) && Aromas.IndexOf(entry.AromaTag) is >= 0
                        ? Aromas.IndexOf(entry.AromaTag) : -1;
                    SelectedTextureIndex = !string.IsNullOrEmpty(entry.TextureTag) && Textures.IndexOf(entry.TextureTag) is >= 0
                        ? Textures.IndexOf(entry.TextureTag) : -1;

                    StarRating = entry.StarRating;
                    OnPropertyChanged(nameof(IsStar1));
                    OnPropertyChanged(nameof(IsStar2));
                    OnPropertyChanged(nameof(IsStar3));
                    OnPropertyChanged(nameof(IsStar4));
                    OnPropertyChanged(nameof(IsStar5));
                    IngredientsText = entry.Ingredients ?? string.Empty;
                    NoteText = entry.NoteText ?? string.Empty;
                    LocationName = entry.LocationName ?? string.Empty;

                    if (DateTime.TryParse(entry.DiscoverDate, out var dt))
                        DiscoverDate = dt;

                    // Restore collection selection
                    SelectedCollections.Clear();
                    if (!string.IsNullOrWhiteSpace(entry.CollectionName))
                    {
                        SelectedCollections.Add(entry.CollectionName);
                    }
                }
            }
            else
            {
                // Create mode
                IsNew = true;
                Title = "New Entry";
                Entry = new FoodEntry();
                SelectedPriceIndex = 1; // Default 10~30
                StarRating = 3;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryEditViewModel] LoadEntry error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(Entry.Name))
            {
                await Application.Current!.MainPage!.DisplayAlert("Notice", "Please enter the food name", "OK");
                return;
            }

            // Fill properties
            Entry.ImagePath = string.IsNullOrWhiteSpace(ImagePath) ? null : ImagePath;
            Entry.Region = SelectedRegionIndex >= 0 ? Regions[SelectedRegionIndex] : "Sichuan";
            Entry.Rarity = SelectedRarityIndex >= 0 ? Rarities[SelectedRarityIndex] : "Common";
            Entry.PriceRange = SelectedPriceIndex >= 0 ? PriceRanges[SelectedPriceIndex] : "10~30";
            Entry.PrimaryTaste = SelectedTasteIndex >= 0 ? Tastes[SelectedTasteIndex] : null;
            Entry.AromaTag = SelectedAromaIndex >= 0 ? Aromas[SelectedAromaIndex] : null;
            Entry.TextureTag = SelectedTextureIndex >= 0 ? Textures[SelectedTextureIndex] : null;
            Entry.StarRating = StarRating;
            Entry.Ingredients = IngredientsText;
            Entry.NoteText = string.IsNullOrWhiteSpace(NoteText) ? null : NoteText;
            Entry.LocationName = string.IsNullOrWhiteSpace(LocationName) ? null : LocationName;
            Entry.DiscoverDate = DiscoverDate?.ToString("yyyy-MM-dd");

            // Collection (take the first selected one)
            var checkedCollection = Collections.FirstOrDefault(c => c.IsChecked);
            Entry.CollectionName = checkedCollection?.Name;

            Entry.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (IsNew)
            {
                // Auto-generate catalog number
                Entry.CatalogNumber = await _foodEntryRepo.GenerateCatalogNumberAsync();
                Entry.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                await _foodEntryRepo.InsertAsync(Entry);
            }
            else
            {
                await _foodEntryRepo.UpdateAsync(Entry);
            }

            await _hapticService.LightAsync();

            // Navigate back
            await Shell.Current.Navigation.PopAsync(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryEditViewModel] Save error: {ex.Message}");
            try
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "Save failed, please try again", "OK");
            }
            catch { }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        try
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryEditViewModel] Cancel error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task TakePhoto()
    {
        try
        {
            var result = await _cameraService.CapturePhotoAsync();
            if (result is not null)
            {
                var path = await _cameraService.SaveToFileAsync(result);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    ImagePath = path;
                    Entry.ImagePath = path;
                    OnPropertyChanged(nameof(HasImage));
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryEditViewModel] TakePhoto error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task PickPhoto()
    {
        try
        {
            var result = await _cameraService.PickPhotoAsync();
            if (result is not null)
            {
                var path = await _cameraService.SaveToFileAsync(result);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    ImagePath = path;
                    Entry.ImagePath = path;
                    OnPropertyChanged(nameof(HasImage));
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryEditViewModel] PickPhoto error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ToggleFlash()
    {
        try
        {
            IsFlashOn = !IsFlashOn;
            await _cameraService.SetFlashAsync(IsFlashOn);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryEditViewModel] ToggleFlash error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task VoiceIngredients()
    {
        if (IsListening) return;
        IsListening = true;
        try
        {
            var result = await _speechService.RecognizeAsync();
            if (!string.IsNullOrWhiteSpace(result))
            {
                IngredientsText = string.IsNullOrWhiteSpace(IngredientsText)
                    ? result
                    : $"{IngredientsText},{result}";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryEditViewModel] VoiceIngredients error: {ex.Message}");
        }
        finally
        {
            IsListening = false;
        }
    }

    [RelayCommand]
    private async Task VoiceNote()
    {
        if (IsListening) return;
        IsListening = true;
        try
        {
            var result = await _speechService.RecognizeAsync();
            if (!string.IsNullOrWhiteSpace(result))
            {
                NoteText = string.IsNullOrWhiteSpace(NoteText)
                    ? result
                    : $"{NoteText}{result}";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryEditViewModel] VoiceNote error: {ex.Message}");
        }
        finally
        {
            IsListening = false;
        }
    }

    [RelayCommand]
    private void SetRating(int rating)
    {
        StarRating = Math.Clamp(rating, 1, 5);
        OnPropertyChanged(nameof(IsStar1));
        OnPropertyChanged(nameof(IsStar2));
        OnPropertyChanged(nameof(IsStar3));
        OnPropertyChanged(nameof(IsStar4));
        OnPropertyChanged(nameof(IsStar5));
    }

    [RelayCommand]
    private async Task GetLocation()
    {
        IsLocating = true;
        try
        {
            var location = await _geolocationService.GetCurrentLocationAsync();
            if (location is not null)
            {
                Entry.Latitude = location.Latitude;
                Entry.Longitude = location.Longitude;
                OnPropertyChanged(nameof(Entry));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryEditViewModel] GetLocation error: {ex.Message}");
        }
        finally
        {
            IsLocating = false;
        }
    }

    public void ApplyPreset(string? name, string? imagePath)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
        if (!string.IsNullOrWhiteSpace(imagePath))
        {
            ImagePath = imagePath;
            Entry.ImagePath = imagePath;
            OnPropertyChanged(nameof(HasImage));
        }
    }
}
