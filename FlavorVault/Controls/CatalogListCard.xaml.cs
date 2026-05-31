using System.Windows.Input;
using FlavorVault.Models;

namespace FlavorVault.Controls;

public partial class CatalogListCard : ContentView
{
    public CatalogListCard()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 绑定属性：FoodEntry 数据
    /// </summary>
    public static readonly BindableProperty EntryProperty =
        BindableProperty.Create(
            nameof(Entry),
            typeof(FoodEntry),
            typeof(CatalogListCard),
            default(FoodEntry));

    public FoodEntry Entry
    {
        get => (FoodEntry)GetValue(EntryProperty);
        set => SetValue(EntryProperty, value);
    }

    /// <summary>
    /// 点击卡片命令
    /// </summary>
    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(
            nameof(TapCommand),
            typeof(ICommand),
            typeof(CatalogListCard),
            default(ICommand));

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }
}
