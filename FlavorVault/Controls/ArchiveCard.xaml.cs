namespace FlavorVault.Controls;

public partial class ArchiveCard : ContentView
{
    public ArchiveCard()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TotalEntriesProperty =
        BindableProperty.Create(
            nameof(TotalEntries),
            typeof(int),
            typeof(ArchiveCard),
            0);

    public int TotalEntries
    {
        get => (int)GetValue(TotalEntriesProperty);
        set => SetValue(TotalEntriesProperty, value);
    }

    public static readonly BindableProperty CollectedCountProperty =
        BindableProperty.Create(
            nameof(CollectedCount),
            typeof(int),
            typeof(ArchiveCard),
            0);

    public int CollectedCount
    {
        get => (int)GetValue(CollectedCountProperty);
        set => SetValue(CollectedCountProperty, value);
    }

    public static readonly BindableProperty WantToTryCountProperty =
        BindableProperty.Create(
            nameof(WantToTryCount),
            typeof(int),
            typeof(ArchiveCard),
            0);

    public int WantToTryCount
    {
        get => (int)GetValue(WantToTryCountProperty);
        set => SetValue(WantToTryCountProperty, value);
    }

    public static readonly BindableProperty ShowcaseCountProperty =
        BindableProperty.Create(
            nameof(ShowcaseCount),
            typeof(int),
            typeof(ArchiveCard),
            0);

    public int ShowcaseCount
    {
        get => (int)GetValue(ShowcaseCountProperty);
        set => SetValue(ShowcaseCountProperty, value);
    }

    public static readonly BindableProperty CollectionCountProperty =
        BindableProperty.Create(
            nameof(CollectionCount),
            typeof(int),
            typeof(ArchiveCard),
            0);

    public int CollectionCount
    {
        get => (int)GetValue(CollectionCountProperty);
        set => SetValue(CollectionCountProperty, value);
    }

    public static readonly BindableProperty PlaceMarkCountProperty =
        BindableProperty.Create(
            nameof(PlaceMarkCount),
            typeof(int),
            typeof(ArchiveCard),
            0);

    public int PlaceMarkCount
    {
        get => (int)GetValue(PlaceMarkCountProperty);
        set => SetValue(PlaceMarkCountProperty, value);
    }
}
