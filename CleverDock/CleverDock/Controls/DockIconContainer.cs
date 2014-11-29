using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using CleverDock.Decorators;
using CleverDock.Managers;
using CleverDock.Model;
using CleverDock.Tools;

namespace CleverDock.Controls
{
    public class DockIconContainer : DockPanel
    {
        public DockIconContainer()
        {
            new FileDropDecorator(this);
            new IconDragDecorator(this);
            new WindowManagerDecorator(this);
            UseLayoutRounding = true;
            Background = Brushes.Transparent;
            Application.Current.MainWindow.Topmost = true;
        }

        public void LoadSettings()
        {
            foreach (SerializableIconInfo info in SettingsManager.Settings.Icons)
            {
                IconInfo i = new IconInfo(info);
                i.Pinned = true;
                Children.Add(new DockIcon(i));
            }
        }

        public void SaveSettings()
        {
            SettingsManager.Settings.Icons =
                    Children.OfType<DockIcon>().Where(i => i.Info.Pinned).Select(i => new SerializableIconInfo(i.Info)).ToList();
            SettingsManager.SaveSettings();
        }

        public bool IsPositionWithinBounds(Point cpos)
        {
            return cpos.X >= 0 && cpos.Y >= 0 && cpos.X <= ActualWidth && cpos.Y <= ActualHeight;
        }

        public int CountIconsBefore(int index)
        {
            int count = 0;
            for (int i = 0; i < index; i++)
                if (Children[i] is DockIcon)
                    count++;
            return count;
        }

        public int GetDropIndex(double x)
        {
            double additive = 0;
            int last = -1;
            int index = 0;
            foreach (Control control in Children)
            {
                if (control is DockIcon)
                {
                    if (x < additive + control.Width/2) // If mouse is before icon (before half width)
                        return last + 1; // Set index after last icon
                    if (x < additive + control.Width) // If mouse is before icon end (after half width)
                        return index + 1; // Set index after current index
                    last = index; // Set last icon index
                }
                additive += control.Width;
                index++;
            }
            return last + 1;
        }
    }
}