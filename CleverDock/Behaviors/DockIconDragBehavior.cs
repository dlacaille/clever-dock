using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CleverDock.Handlers;
using CleverDock.Managers;
using CleverDock.Patterns;
using CleverDock.Tools;
using CleverDock.ViewModels;
using Microsoft.Xaml.Behaviors;
using MouseButtonEventArgs = System.Windows.Input.MouseButtonEventArgs;

namespace CleverDock.Behaviors;

public class DockIconDragBehavior : Behavior<FrameworkElement>
{
    private DraggedIconWindow draggedIconWindow;
    private IconViewModel icon;
    private bool isMouseDown;
    private ItemsControl itemsControl;
    private Point mouseStart;
    private double mouseX, mouseY;
    private IconViewModel placeholder;

    private Panel Panel => FrameworkHelper.GetVisualChild<Panel>(itemsControl);

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
        AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
        AssociatedObject.MouseMove += AssociatedObject_MouseMove;
        AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
    }

    private void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
    {
        DetachIcon();
    }

    private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
    {
        var pos = e.GetPosition(AssociatedObject);
        if (Distance(mouseStart, pos) > 10)
            DetachIcon();
    }

    private double Distance(Point a, Point b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }

    private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        mouseStart = e.GetPosition(AssociatedObject);
        isMouseDown = true;
    }

    private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        isMouseDown = false;
    }

    private void DetachIcon()
    {
        if (isMouseDown)
        {
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
            itemsControl = FrameworkHelper.GetParent<ItemsControl>(AssociatedObject);
            MouseManager.Manager.MouseMoved += Manager_MouseMoved;
            icon = (IconViewModel)AssociatedObject.DataContext;
            VMLocator.Main.Icons.Remove(icon);
            if (draggedIconWindow == null)
                draggedIconWindow = new DraggedIconWindow();
            draggedIconWindow.DataContext = icon;
            itemsControl.CaptureMouse();
            itemsControl.MouseUp += itemsControl_MouseUp;
            itemsControl.MouseMove += itemsControl_MouseMove;
        }
    }

    private void itemsControl_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (IsInDock(mouseX, mouseY))
        {
            // Replace placeholder with icon.
            var index = VMLocator.Main.Icons.IndexOf(placeholder);
            VMLocator.Main.Icons.Remove(placeholder);
            VMLocator.Main.Icons.Insert(index, icon);
            icon.Pinned = true;
        }
        else
        {
            VMLocator.Main.Icons.Remove(placeholder);
            if (icon.IsActive)
                VMLocator.Main.Icons.Add(icon);
        }

        // Clean up events and variables.
        isMouseDown = false;
        MouseManager.Manager.MouseMoved -= Manager_MouseMoved;
        itemsControl.ReleaseMouseCapture();
        itemsControl.MouseUp -= itemsControl_MouseUp;
        itemsControl.MouseMove -= itemsControl_MouseMove;
        var window = draggedIconWindow;
        AnimationTools.FadeOut(0.2, window, 0, () =>
        {
            window.Close();
            window = null;
        });
        icon = null;
        placeholder = null;
    }

    private bool IsInElement(double x, double y)
    {
        return x >= 0 && x <= AssociatedObject.ActualWidth && y >= 0 && y <= AssociatedObject.ActualHeight;
    }

    private bool IsInDock(double x, double y)
    {
        return x >= 0 && x <= itemsControl.ActualWidth && y >= 0 && y <= itemsControl.ActualHeight;
    }

    private void itemsControl_MouseMove(object sender, MouseEventArgs e)
    {
        mouseY = e.GetPosition(itemsControl).Y;
        mouseX = e.GetPosition(itemsControl).X;
        if (IsInDock(mouseX, mouseY))
        {
            var index = GetDropIndex(mouseX);
            if (placeholder == null)
                placeholder = new IconViewModel();
            if (VMLocator.Main.Icons.IndexOf(placeholder) != index)
            {
                VMLocator.Main.Icons.Remove(placeholder);
                VMLocator.Main.Icons.Insert(index, placeholder);
            }
        }
        else
        {
            VMLocator.Main.Icons.Remove(placeholder);
            placeholder = null;
        }
    }

    public FrameworkElement FindFrameworkElement(IconViewModel icon)
    {
        return Panel.Children.OfType<FrameworkElement>().FirstOrDefault(c => c.DataContext == icon);
    }

    public int GetDropIndex(double x)
    {
        double cumul = 0;
        var index = 0;
        foreach (FrameworkElement item in Panel.Children)
        {
            var width = item.ActualWidth;
            if (x < cumul + width) // If mouse is before icon (before half width)
                return index; // Set index after last icon
            cumul += width;
            index++;
        }

        return index - 1;
    }

    private void Manager_MouseMoved(object sender, MouseMoveEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var iconSize = VMLocator.Main.IconSize;
            draggedIconWindow.Left = e.CursorPosition.X - iconSize / 2;
            draggedIconWindow.Top = e.CursorPosition.Y - iconSize / 2;
            if (!draggedIconWindow.IsActive)
                draggedIconWindow.Show();
        });
    }
}