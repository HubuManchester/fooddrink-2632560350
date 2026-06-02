using System.Windows.Input;

namespace FlavorVault.Controls;

public partial class CatalogPhotoCard : ContentView
{
    public CatalogPhotoCard()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(
            nameof(TapCommand),
            typeof(ICommand),
            typeof(CatalogPhotoCard),
            default(ICommand));

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }
}
