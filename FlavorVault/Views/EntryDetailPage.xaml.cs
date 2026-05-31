using FlavorVault.ViewModels;

namespace FlavorVault.Views;

[QueryProperty(nameof(EntryId), "id")]
public partial class EntryDetailPage : ContentPage
{
    private string? _entryId;

    public string? EntryId
    {
        get => _entryId;
        set
        {
            _entryId = value;
            if (BindingContext is EntryDetailViewModel vm && int.TryParse(value, out int id))
            {
                vm.LoadEntryCommand.Execute(id);
            }
        }
    }

    public EntryDetailPage()
    {
        InitializeComponent();
        BindingContext = App.Current?.Handler?.MauiContext?.Services?.GetService<EntryDetailViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is EntryDetailViewModel vm && int.TryParse(EntryId, out int id))
        {
            await vm.LoadEntryCommand.ExecuteAsync(id);
        }
    }
}
