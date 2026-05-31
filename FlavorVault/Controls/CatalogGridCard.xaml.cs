using System.Windows.Input;
using FlavorVault.Models;

namespace FlavorVault.Controls;

public partial class CatalogGridCard : ContentView
{
    public CatalogGridCard()
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
            typeof(CatalogGridCard),
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
            typeof(CatalogGridCard),
            default(ICommand));

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }
}
