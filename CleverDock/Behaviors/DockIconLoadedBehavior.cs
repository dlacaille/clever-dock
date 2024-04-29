using System.Windows;
using CleverDock.ViewModels;
using Microsoft.Xaml.Behaviors;

namespace CleverDock.Behaviors;

public class DockIconLoadedBehavior : Behavior<FrameworkElement>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        var icon = (IconViewModel)AssociatedObject.DataContext;
        icon.Element = AssociatedObject;
    }
}