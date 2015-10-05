using CleverDock.ViewModels;
using System.Windows;
using System.Windows.Interactivity;

namespace CleverDock.Behaviors
{
    public class DockIconLoadedBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            IconViewModel icon =  (IconViewModel)AssociatedObject.DataContext;
            icon.Element = AssociatedObject;
        }
    }
}
