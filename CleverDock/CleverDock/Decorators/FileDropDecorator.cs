using System;
using System.IO;
using System.Windows;
using CleverDock.Controls;
using CleverDock.Managers;
using CleverDock.Model;

namespace CleverDock.Decorators
{
    public class FileDropDecorator
    {
        private DockIconContainer container;

        private bool dragInProgress;
        private int dragIndex = -1;
        private DockIcon dragItem;

        public FileDropDecorator(DockIconContainer _container)
        {
            container = _container;
            container.AllowDrop = true;
            container.DragEnter += DockIconContainer_DragEnter;
            container.Drop += DockIconContainer_Drop;
            container.DragOver += DockIconContainer_DragOver;
            container.DragLeave += DockIconContainer_DragLeave;
        }

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
                dragItem = new DockIcon(new IconInfo
                {
                        Path = files[0],
                        Name = Path.GetFileNameWithoutExtension(files[0])
                });
                dragIndex = container.GetDropIndex(e.GetPosition(container).X - SettingsManager.Settings.OuterIconSize/2);
                container.Children.Insert(dragIndex, dragItem);
            }
        }

        private void DockIconContainer_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            if (dragItem != null && dragIndex > 0 && SettingsManager.Settings.SaveAutomatically)
            {
                dragItem.Info.Pinned = true;
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
                container.GetDropIndex(e.GetPosition(container).X - SettingsManager.Settings.OuterIconSize/2))
            {
                if (dragIndex >= 0)
                    container.Children.Remove(dragItem);
                int index = container.GetDropIndex(e.GetPosition(container).X - SettingsManager.Settings.OuterIconSize/2);
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