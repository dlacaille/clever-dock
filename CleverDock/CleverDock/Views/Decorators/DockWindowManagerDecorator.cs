using CleverDock.Managers;
using CleverDock.Model;
using CleverDock.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CleverDock.Views.Decorators
{
    public class DockWindowManagerDecorator
    {
        private Dock dock { get; set; }

        public DockWindowManagerDecorator(Dock dock)
        {
            this.dock = dock;
            Application.Current.Exit += Current_Exit;
            WindowManager.Manager.WindowAdded += Manager_WindowAdded;
            WindowManager.Manager.WindowRemoved += Manager_WindowRemoved;
            WindowManager.Manager.Start();
            //TaskbarManager.SetTaskbarVisibility(false);
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            //TaskbarManager.SetTaskbarVisibility(true);
        }

        void Manager_WindowAdded(object sender, Handlers.WindowEventArgs e)
        {
            var foundItems = dock.Icons.Where(icon => PathTools.SamePath(icon.Info.Path, e.Window.FileName));
            DockIcon windowIcon = null;
            if (foundItems.Any())
                windowIcon = foundItems.First();
            if (!foundItems.Any() || (windowIcon != null && windowIcon.Windows.Count > 0))
            {
                windowIcon = new DockIcon(new IconInfo()
                {
                    Name = Path.GetFileName(e.Window.FileName),
                    Path = e.Window.FileName,
                    Pinned = false
                });
                dock.Subviews.Add(windowIcon);
            }
            windowIcon.Windows.Add(e.Window);
        }

        void Manager_WindowRemoved(object sender, Handlers.WindowEventArgs e)
        {
            var foundItem = dock.Icons.First(i => i.Windows.Any(w => w.Hwnd == e.Window.Hwnd));
            foundItem.Windows.Remove(e.Window);
            if (!foundItem.Windows.Any() && !foundItem.Info.Pinned)
                dock.Subviews.Remove(foundItem);
        }
    }
}
