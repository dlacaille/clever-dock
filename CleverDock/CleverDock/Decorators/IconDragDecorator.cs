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

        private DockIcon draggedIcon;
        private Image draggedIconImage;
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
            placeholder = new Separator {Width = SettingsManager.Settings.OuterIconSize, Visibility = Visibility.Hidden};
            container.Children.Insert(index, placeholder);

            draggedIconImage = draggedIcon.IconImage;
            draggedIcon.IconGrid.Children.Remove(draggedIconImage);
            canvas.Children.Add(draggedIconImage);
        }

        private void MoveDraggedIcon(Point pos)
        {
            draggedIconImage.SetValue(Canvas.LeftProperty, pos.X - draggedIconImage.Width/2 - offset.X);
            draggedIconImage.SetValue(Canvas.TopProperty, pos.Y - draggedIconImage.Height/2 - offset.Y);
        }

        private void PlaceDraggedIcon()
        {
            int index = container.Children.IndexOf(placeholder);
            container.Children.Remove(placeholder);
            canvas.Children.Remove(draggedIconImage);
            var windows = draggedIcon.Windows;
            draggedIcon = new DockIcon(draggedIcon.Info);
            foreach(var w in windows)
                draggedIcon.Windows.Add(w);
            container.Children.Insert(index, draggedIcon);
        }

        private void Dispose()
        {
            MouseDown = false;
            IsDragging = false;
            draggedIcon = null;
            draggedIconImage = null;
            placeholder = null;
            startPos = new Point(0, 0);
            offset = new Point(0, 0);
            lastCount = -1;
        }

        public void AddPlaceholder(int index)
        {
            placeholder = new Separator {Width = 0, Visibility = Visibility.Hidden};
            container.Children.Insert(index, placeholder);
        }

        public void RemovePlaceholder()
        {
            Separator p = placeholder;
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

        private void DockIconContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedIcon == null)
                return;
            Point pos = e.GetPosition(canvas);
            Point cpos = e.GetPosition(container);
            int iconsize = SettingsManager.Settings.OuterIconSize;
            if (!IsDragging && GetDistance(e.GetPosition(null), startPos) > minDistance)
            {
                IsDragging = true;
                DetachDraggedIcon();
                offset.X = cpos.X % iconsize - iconsize /2;
                offset.Y = cpos.Y - container.ActualHeight / 2;
            }
            if (IsDragging)
            {
                MoveDraggedIcon(pos);
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