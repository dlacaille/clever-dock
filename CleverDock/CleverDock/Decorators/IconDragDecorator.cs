using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CleverDock.Controls;
using CleverDock.Managers;
using CleverDock.Tools;
using Window = CleverDock.Model.Window;

namespace CleverDock.Decorators
{
    public class IconDragDecorator
    {
        private static float minDistance = 5;
        private bool IsDragging;
        private bool MouseDown;
        private DockIconContainer container;
        private DraggedIconWindow draggedIconWindow;

        private DockIcon draggedIcon;
        private int lastCount = -1;
        private Point offset;
        private Separator placeholder;
        private Point startPos;

        public IconDragDecorator(DockIconContainer _container)
        {
            container = _container;
            _container.MouseLeftButtonDown += DockIconContainer_MouseDown;
            _container.MouseMove += DockIconContainer_MouseMove;
            _container.MouseLeftButtonUp += DockIconContainer_MouseUp;
            _container.LostMouseCapture += DockIconContainer_LostMouseCapture;
            WindowManager.Manager.CursorPositionChanged += Manager_CursorPositionChanged;
        }

        private Canvas canvas
        {
            get { return (Canvas) container.Parent; }
        }

        private DockIcon GetSelectedIcon()
        {
            return (from DockIcon i in container.Children
                    where i.IsMouseOver
                    select i).FirstOrDefault();
        }

        private void DetachDraggedIcon()
        {
            int index = container.Children.IndexOf(draggedIcon);
            container.Children.Remove(draggedIcon);

            lastCount = container.CountIconsBefore(index);
            placeholder = new Separator {Width = SettingsManager.Settings.OuterIconWidth, Visibility = Visibility.Hidden};
            container.Children.Insert(index, placeholder);

            if (draggedIconWindow == null)
                draggedIconWindow = new DraggedIconWindow();
            draggedIconWindow.Width = 
                draggedIconWindow.IconImage.Width = 
                draggedIconWindow.Height = 
                draggedIconWindow.IconImage.Height = 
                    SettingsManager.Settings.IconSize;
            draggedIconWindow.Topmost = true;
            draggedIconWindow.IconImage.Source = draggedIcon.IconImage.Source;
            draggedIconWindow.Show();
        }

        private void PlaceDraggedIcon()
        {
            int index = container.Children.IndexOf(placeholder);
            container.Children.Remove(placeholder);
            var windows = draggedIcon.Windows;
            draggedIcon = new DockIcon(draggedIcon.Info);
            foreach(var w in windows)
                draggedIcon.Windows.Add(w);
            container.Children.Insert(index, draggedIcon);
            RemoveDraggedIcon();
        }

        private void RemoveDraggedIcon()
        {
            draggedIconWindow.Close();
            draggedIconWindow = null;
        }

        private void Dispose()
        {
            MouseDown = false;
            IsDragging = false;
            draggedIcon = null;
            placeholder = null;
            startPos = new Point(0, 0);
            offset = new Point(0, 0);
            lastCount = -1;
        }

        public void AddPlaceholder(int index)
        {
            placeholder = new Separator {Width = 0, Visibility = Visibility.Hidden};
            container.Children.Insert(index, placeholder);
            AnimationTools.ExpandX(SettingsManager.Settings.CollapseDuration, SettingsManager.Settings.OuterIconWidth, placeholder, null, 0.1);
        }

        public void RemovePlaceholder()
        {
            Separator p = placeholder;
            AnimationTools.CollapseX(SettingsManager.Settings.CollapseDuration, p, () => {
                container.Children.Remove(p);
            });
            placeholder = null;
        }

        public static double GetDistance(Point p, Point q)
        {
            double a = p.X - q.X;
            double b = p.Y - q.Y;
            return Math.Sqrt(a*a + b*b);
        }

        private void DockIconContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseDown = true;
            startPos = e.GetPosition(null);
            draggedIcon = GetSelectedIcon();
            if (draggedIcon != null)
                container.CaptureMouse();
        }

        void Manager_CursorPositionChanged(object sender, Handlers.CursorPosEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (IsDragging)
                {
                    var iconSize = SettingsManager.Settings.IconSize;
                    draggedIconWindow.Left = e.CursorPosition.X - iconSize / 2;
                    draggedIconWindow.Top = e.CursorPosition.Y - iconSize / 2;
                }
            });
        }

        private void DockIconContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedIcon == null)
                return;
            Point pos = e.GetPosition(canvas);
            Point cpos = e.GetPosition(container);
            int iconsize = SettingsManager.Settings.OuterIconWidth;
            if (!IsDragging && GetDistance(e.GetPosition(null), startPos) > minDistance)
            {
                IsDragging = true;
                DetachDraggedIcon();
                offset.X = cpos.X % iconsize - iconsize /2;
                offset.Y = cpos.Y - container.ActualHeight / 2;
            }
            if (IsDragging)
            {
                if (container.IsPositionWithinBounds(cpos))
                {
                    int dropIndex = container.GetDropIndex(cpos.X);
                    int count = container.CountIconsBefore(dropIndex);
                    if (lastCount != count)
                    {
                        if (placeholder != null)
                            RemovePlaceholder();
                        AddPlaceholder(dropIndex);
                        lastCount = count;
                    }
                }
                else if (placeholder != null)
                {
                    RemovePlaceholder();
                    lastCount = -1;
                }
            }
        }

        private void DockIconContainer_LostMouseCapture(object sender, MouseEventArgs e)
        {
            MouseReleased(e.GetPosition(container));
        }

        private void DockIconContainer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseReleased(e.GetPosition(container));
        }

        private void MouseReleased(Point pos)
        {
            if (draggedIcon == null || !MouseDown)
                return;
            MouseDown = false;
            if (IsDragging)
            {
                if (container.IsPositionWithinBounds(pos))
                {
                    draggedIcon.Info.Pinned = true;
                    PlaceDraggedIcon();
                }
                else
                {
                    RemoveDraggedIcon();
                    if(draggedIcon.Windows.Any())
                    {
                        DockIcon icon = new DockIcon(draggedIcon.Info);
                        foreach(Window w in draggedIcon.Windows)
                            icon.Windows.Add(w);
                        icon.Info.Pinned = false;
                        container.Children.Add(icon);
                    }
                }
                if (SettingsManager.Settings.SaveAutomatically)
                    container.SaveSettings();
            }
            else
                draggedIcon.Run();
            Dispose();
            container.ReleaseMouseCapture();
        }
    }
}