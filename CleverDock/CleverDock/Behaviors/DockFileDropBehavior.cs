using CleverDock.Managers;
using CleverDock.Patterns;
using CleverDock.Tools;
using CleverDock.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace CleverDock.Behaviors
{
    public class DockFileDropBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.DragEnter += DockIconContainer_DragEnter;
            AssociatedObject.Drop += DockIconContainer_Drop;
            AssociatedObject.DragOver += DockIconContainer_DragOver;
            AssociatedObject.DragLeave += DockIconContainer_DragLeave;
        }
        private DockIconContainer container;

        private bool dragInProgress;
        private int dragIndex = -1;
        private DockIcon dragItem;

        private void DockIconContainer_DragLeave(object sender, DragEventArgs e)
        {
            dragInProgress = false;
            container.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (dragInProgress)
                    return;
                if (dragIndex > -1)
                    container.Children.Remove(dragItem);
                EndDrag();
            }));
        }

        private void DockIconContainer_DragEnter(object sender, DragEventArgs e)
        {
            dragInProgress = true;
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            // Note that you can have more than one file.
            if (dragIndex == -1)
            {
                var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
                dragItem = new DockIcon(new IconModel
                {
                    Path = files[0],
                    Name = Path.GetFileNameWithoutExtension(files[0])
                });
                dragIndex = container.GetDropIndex(e.GetPosition(container).X - VMLocator.Main.OuterIconWidth / 2);
                container.Children.Insert(dragIndex, dragItem);
            }
        }

        private void DockIconContainer_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            if (dragItem != null && dragIndex > 0 && VMLocator.Main.SaveAutomatically)
            {
                dragItem.Pinned = true;
                container.SaveSettings();
            }
            EndDrag();
        }

        private void DockIconContainer_DragOver(object sender, DragEventArgs e)
        {
            dragInProgress = true;
            e.Effects = DragDropEffects.Copy;
            e.Handled = false;
            if (dragIndex !=
                container.GetDropIndex(e.GetPosition(container).X - SettingsManager.Settings.OuterIconWidth / 2))
            {
                if (dragIndex >= 0)
                    container.Children.Remove(dragItem);
                int index = container.GetDropIndex(e.GetPosition(container).X - SettingsManager.Settings.OuterIconWidth / 2);
                container.Children.Insert(index, dragItem);
                dragItem.InvalidateArrange();
                dragIndex = index;
            }
        }

        private void EndDrag()
        {
            dragItem = null;
            dragIndex = -1;
        }
    }
}
