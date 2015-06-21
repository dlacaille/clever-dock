using CleverDock.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace CleverDock.Behaviors
{
    public class DockIconMinimizeBehavior : Behavior<FrameworkElement>
    {
        private IconViewModel icon;

        protected override void OnAttached()
        {
            base.OnAttached();
            icon = (IconViewModel)AssociatedObject.DataContext;
            icon.OnMinimize += icon_OnMinimize;
        }

        void icon_OnMinimize(object sender, EventArgs e)
        {

        }
    }
}
