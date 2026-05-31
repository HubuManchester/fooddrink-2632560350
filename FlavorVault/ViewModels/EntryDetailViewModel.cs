using FlavorVault.Models;
using FlavorVault.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlavorVault.ViewModels;

/// <summary>
/// 图鉴条目详情 ViewModel，支持朗读、橱窗切换和收藏状态变更
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
    /// 主要食材显示文本
    /// </summary>
    public string IngredientsText => Entry?.Ingredients ?? string.Empty;

    /// <summary>
    /// 坐标文本
    /// </summary>
    public string CoordinatesText =>
        Entry?.LocationName != null
            ? $"{Entry.Latitude:F6}, {Entry.Longitude:F6}"
            : string.Empty;

    /// <summary>
    /// 朗读按钮文本
    /// </summary>
    public string SpeakButtonText => IsSpeaking ? "停止" : "朗读";

    /// <summary>
    /// 收藏状态按钮文本
    /// </summary>
    public string CollectStatusText => Entry?.CollectStatus ?? "已收藏";

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

                // 加载关联收藏集名称
                if (!string.IsNullOrWhiteSpace(entry.CollectionName))
                {
                    CollectionNames = entry.CollectionName;
                }
                else
                {
                    CollectionNames = "未归入收藏集";
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryDetailViewModel] LoadEntry 错误: {ex.Message}");
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

            // 振动反馈
            await _hapticService.LightAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryDetailViewModel] ToggleShowcase 错误: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[EntryDetailViewModel] GoBack 错误: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[EntryDetailViewModel] Edit 错误: {ex.Message}");
        }
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task ToggleSpeaking()
    {
        try
        {
            if (IsSpeaking)
            {
                // 停止朗读
                await _ttsService.CancelAsync();
                IsSpeaking = false;
            }
            else
            {
                // 开始朗读
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
            System.Diagnostics.Debug.WriteLine($"[EntryDetailViewModel] ToggleSpeaking 错误: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ChangeCollectStatus()
    {
        try
        {
            // 循环切换：已收藏 → 想尝试 → 已尝试 → 已收藏
            Entry.CollectStatus = Entry.CollectStatus switch
            {
                "已收藏" => "想尝试",
                "想尝试" => "已尝试",
                "已尝试" => "已收藏",
                _ => "已收藏"
            };

            Entry.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            await _foodEntryRepo.UpdateAsync(Entry);
            OnPropertyChanged(nameof(Entry));
            OnPropertyChanged(nameof(CollectStatusText));

            await _hapticService.LightAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EntryDetailViewModel] ChangeCollectStatus 错误: {ex.Message}");
        }
    }
}
