using System;
using System.Linq;
using System.Windows.Threading;
using CleverDock.Controls;
using CleverDock.Managers;
using CleverDock.Model;
using CleverDock.Tools;
using System.Windows;
using System.IO;

namespace CleverDock.Decorators
{
    public class WindowManagerDecorator
    {
        private DockIconContainer container;

        public WindowManagerDecorator(DockIconContainer _container)
        {
            container = _container;
            Application.Current.Exit += Current_Exit;
            WindowManager.Manager.WindowAdded += Manager_WindowAdded;
            WindowManager.Manager.WindowRemoved += Manager_WindowRemoved;
            WindowManager.Manager.Start();
            SettingsManager.Settings.PropertyChanged += Settings_PropertyChanged;
            if (SettingsManager.Settings.RemoveTaskbar)
                TaskbarManager.SetTaskbarVisibility(false);
        }

        void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RemoveTaskbar":
                    TaskbarManager.SetTaskbarVisibility(!SettingsManager.Settings.RemoveTaskbar);
                    break;
            }
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            if (SettingsManager.Settings.RemoveTaskbar)
                TaskbarManager.SetTaskbarVisibility(true);
            WindowManager.Manager.Stop();
        }

        void Manager_WindowAdded(object sender, Handlers.WindowEventArgs e)
        {
            container.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                var foundItems = container.Children.OfType<DockIcon>().Select(el => el).Where(
                                 icon => PathTools.SamePath(icon.Info.Path, e.Window.FileName));
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
                    container.Children.Add(windowIcon);
                }
                windowIcon.Windows.Add(e.Window);
            }));
        }

        void Manager_WindowRemoved(object sender, Handlers.WindowEventArgs e)
        {
            container.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                var foundItem =
                        container.Children.OfType<DockIcon>().First(i => i.Windows.Any(w => w.Hwnd == e.Window.Hwnd));
                foundItem.Windows.Remove(e.Window);
                if (!foundItem.Windows.Any() && !foundItem.Info.Pinned)
                    container.Children.Remove(foundItem);
            }));
        }
    }
}    