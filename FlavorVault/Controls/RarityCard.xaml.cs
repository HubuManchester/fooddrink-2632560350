using System.Collections.ObjectModel;
using FlavorVault.Models;

namespace FlavorVault.Controls;

public partial class RarityCard : ContentView
{
    public RarityCard()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty RarityStatsProperty =
        BindableProperty.Create(
            nameof(RarityStats),
            typeof(ObservableCollection<RarityCount>),
            typeof(RarityCard),
            default(ObservableCollection<RarityCount>));

    public ObservableCollection<RarityCount> RarityStats
    {
        get => (ObservableCollection<RarityCount>)GetValue(RarityStatsProperty);
        set => SetValue(RarityStatsProperty, value);
    }
}
