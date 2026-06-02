using FlavorVault.ViewModels;

namespace FlavorVault.Views;

[QueryProperty(nameof(EntryId), "id")]
[QueryProperty(nameof(PresetName), "name")]
[QueryProperty(nameof(PresetImagePath), "imagePath")]
public partial class EntryEditPage : ContentPage
{
    private string? _entryId;
    private string? _presetName;
    private string? _presetImagePath;

    public string? EntryId
    {
        get => _entryId;
        set
        {
            _entryId = value;
            if (BindingContext is EntryEditViewModel vm && int.TryParse(value, out int id))
            {
                vm.LoadEntryCommand.Execute(id);
            }
        }
    }

    public string? PresetName
    {
        get => _presetName;
        set
        {
            _presetName = Uri.UnescapeDataString(value ?? string.Empty);
        }
    }

    public string? PresetImagePath
    {
        get => _presetImagePath;
        set
        {
            _presetImagePath = Uri.UnescapeDataString(value ?? string.Empty);
        }
    }

    public EntryEditPage()
    {
        InitializeComponent();
        BindingContext = App.Current?.Handler?.MauiContext?.Services?.GetService<EntryEditViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is EntryEditViewModel vm)
        {
            if (int.TryParse(EntryId, out int id))
            {
                await vm.LoadEntryCommand.ExecuteAsync(id);
            }
            else
            {
                await vm.LoadEntryCommand.ExecuteAsync(null);

                if (!string.IsNullOrWhiteSpace(PresetName))
                    vm.ApplyPreset(PresetName, PresetImagePath);
            }
        }
    }
}
