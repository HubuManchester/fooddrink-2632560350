using FlavorVault.ViewModels;

namespace FlavorVault.Views;

[QueryProperty(nameof(EntryId), "id")]
public partial class EntryEditPage : ContentPage
{
    private string? _entryId;

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
            }
        }
    }
}
