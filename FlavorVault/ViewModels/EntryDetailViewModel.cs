using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// Entry detail ViewModel, supports TTS, showcase toggle and collect status change
/// </summary>
public partial class EntryDetailViewModel : BaseViewModel
{
    private readonly FoodEntryRepository _foodEntryRepo;
    private readonly CollectionRepository _collectionRepo;
    private readonly TextToSpeechService _ttsService;
    private readonly HapticService _hapticService;

    [ObservableProperty]
    private FoodEntry _entry = new();

    [ObservableProperty]
    private bool _isSpeaking;

    [ObservableProperty]
    private string _collectionNames = string.Empty;

    public EntryDetailViewModel(
        FoodEntryRepository foodEntryRepo,
        CollectionRepository collectionRepo,
        TextToSpeechService ttsService,
        HapticService hapticService)
    {
        _foodEntryRepo = foodEntryRepo;
        _collectionRepo = collectionRepo;
        _ttsService = ttsService;
        _hapticService = hapticService;
    }

    /// <summary>
    /// Main ingredients display text
    /// </summary>
    public string IngredientsText => Entry?.Ingredients ?? string.Empty;

    /// <summary>
    /// Coordinates text
    /// </summary>
    public string CoordinatesText =>
        Entry?.LocationName != null
            ? $"{Entry.Latitude:F6}, {Entry.Longitude:F6}"
            : string.Empty;

    /// <summary>
    /// TTS button text
    /// </summary>
    public string SpeakButtonText => IsSpeaking ? "Stop" : "Read Aloud";

    /// <summary>
    /// Collect status button text
    /// </summary>
    public string CollectStatusText => Entry?.CollectStatus ?? "Collected";

    [RelayCommand]
    private async Task LoadEntry(int id)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var entry = await _foodEntryRepo.GetByIdAsync(id);
            if (entry is not null)
            {
                Entry = entry;
                Title = entry.Name;
                OnPropertyChanged(nameof(IngredientsText));
                OnPropertyChanged(nameof(CoordinatesText));
                OnPropertyChanged(nameof(CollectStatusText));

                // Load associated collection names
                if (!string.IsNullOrWhiteSpace(entry.CollectionName))
                {
                    CollectionNames = entry.CollectionName;
                }
                else
                {
                    CollectionNames = "Not in any collection";
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryDetailViewModel] LoadEntry error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task ToggleShowcase()
    {
        try
        {
            Entry.IsShowcase = !Entry.IsShowcase;
            Entry.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            await _foodEntryRepo.UpdateAsync(Entry);
            OnPropertyChanged(nameof(Entry));

            // Haptic feedback
            await _hapticService.LightAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryDetailViewModel] ToggleShowcase error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        try
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryDetailViewModel] GoBack error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Edit()
    {
        try
        {
            await Shell.Current.GoToAsync($"EntryEditPage?id={Entry.Id}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryDetailViewModel] Edit error: {ex.Message}");
        }
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task ToggleSpeaking()
    {
        try
        {
            if (IsSpeaking)
            {
                // Stop reading
                await _ttsService.CancelAsync();
                IsSpeaking = false;
            }
            else
            {
                // Start reading
                var text = Entry.Description ?? Entry.Name;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    IsSpeaking = true;
                    await _ttsService.SpeakAsync(text);
                    IsSpeaking = false;
                }
            }
            OnPropertyChanged(nameof(SpeakButtonText));
        }
        catch (Exception ex)
        {
            IsSpeaking = false;
            OnPropertyChanged(nameof(SpeakButtonText));
            System.Diagnostics.Debug.WriteLine($"[EntryDetailViewModel] ToggleSpeaking error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ChangeCollectStatus()
    {
        try
        {
            // Cycle through: Collected → WantToTry → Tried → Collected
            Entry.CollectStatus = Entry.CollectStatus switch
            {
                "Collected" => "WantToTry",
                "WantToTry" => "Tried",
                "Tried" => "Collected",
                _ => "Collected"
            };

            Entry.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            await _foodEntryRepo.UpdateAsync(Entry);
            OnPropertyChanged(nameof(Entry));
            OnPropertyChanged(nameof(CollectStatusText));

            await _hapticService.LightAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryDetailViewModel] ChangeCollectStatus error: {ex.Message}");
        }
    }
}
