using System;
using System.Security.Cryptography.X509Certificates;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ControlCatalog.Pages;

public partial class OverlapWindowTestPage : UserControl
{
    private Button _btnShowDialog;
    private Window _childWindow;

    public OverlapWindowTestPage()
    {
        InitializeComponent();
        
        _btnShowDialog = this.Get<Button>("btnShowDialog");
        _btnShowDialog.Click += _btnShowDialog_Click;
    }

    private void _btnShowDialog_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var owner = Window.GetTopLevel(this) as Window;
        if(owner != null)
        {
            owner.PositionChanged -= Owner_PositionChanged;
            owner.PositionChanged += Owner_PositionChanged;
        } 

        var childWindow = new Window();
        childWindow.Title = "ChildWindow";
        childWindow.PositionChanged += ChildWindow_PositionChanged;
        childWindow.Show(owner);
        _childWindow = childWindow;
    }

    private void ChildWindow_PositionChanged(object? sender, PixelPointEventArgs e)
    {
        Console.WriteLine($"Child Window Position Changed {e.Point}");
    }

    private void Owner_PositionChanged(object sender, PixelPointEventArgs e)
    {
        if (_childWindow == null)
            return;

        _childWindow.Position = e.Point;
    }
}
