using System.Collections.ObjectModel;
using FlavorVault.Models;

namespace FlavorVault.Controls;

public partial class ProgressCard : ContentView
{
    public ProgressCard()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty RegionProgressesProperty =
        BindableProperty.Create(
            nameof(RegionProgresses),
            typeof(ObservableCollection<RegionProgress>),
            typeof(ProgressCard),
            default(ObservableCollection<RegionProgress>));

    public ObservableCollection<RegionProgress> RegionProgresses
    {
        get => (ObservableCollection<RegionProgress>)GetValue(RegionProgressesProperty);
        set => SetValue(RegionProgressesProperty, value);
    }
}
