using System.Windows.Input;
using FlavorVault.Models;

namespace FlavorVault.Controls;

public partial class WishCard : ContentView
{
    public WishCard()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 绑定属性：WishItem 数据
    /// </summary>
    public static readonly BindableProperty WishItemProperty =
        BindableProperty.Create(
            nameof(WishItem),
            typeof(WishItem),
            typeof(WishCard),
            default(WishItem));

    public WishItem WishItem
    {
        get => (WishItem)GetValue(WishItemProperty);
        set => SetValue(WishItemProperty, value);
    }

    /// <summary>
    /// 切换完成状态命令
    /// </summary>
    public static readonly BindableProperty ToggleCompleteCommandProperty =
        BindableProperty.Create(
            nameof(ToggleCompleteCommand),
            typeof(ICommand),
            typeof(WishCard),
            default(ICommand));

    public ICommand ToggleCompleteCommand
    {
        get => (ICommand)GetValue(ToggleCompleteCommandProperty);
        set => SetValue(ToggleCompleteCommandProperty, value);
    }
}
