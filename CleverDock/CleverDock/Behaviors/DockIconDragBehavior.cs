using CleverDock.Managers;
using CleverDock.Patterns;
using CleverDock.Tools;
using CleverDock.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace CleverDock.Behaviors
{
    public class DockIconDragBehavior : Behavior<FrameworkElement>
    {
        private DraggedIconWindow draggedIconWindow;
        private bool isMouseDown = false;
        private ItemsControl itemsControl;
        private IconViewModel icon;
        private IconViewModel placeholder;
        private double mouseX, mouseY;

        private Panel Panel 
        {
            get
            {
                return FrameworkHelper.GetVisualChild<Panel>(itemsControl);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
        }

        void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isMouseDown = true;
        }

        void AssociatedObject_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isMouseDown = false;
        }

        void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isMouseDown)
            {
                itemsControl = FrameworkHelper.GetParent<ItemsControl>(AssociatedObject);
                WindowManager.Manager.CursorPositionChanged += Manager_CursorPositionChanged;
                icon = (IconViewModel)AssociatedObject.DataContext;
                VMLocator.Main.Icons.Remove(icon);
                if (draggedIconWindow == null)
                    draggedIconWindow = new DraggedIconWindow();
                draggedIconWindow.DataContext = icon;
                itemsControl.CaptureMouse();
                itemsControl.MouseUp += itemsControl_MouseUp;
                itemsControl.MouseMove += itemsControl_MouseMove;
            }
        }

        void itemsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsInDock(mouseX, mouseY))
            {
                // Replace placeholder with icon.
                var index = VMLocator.Main.Icons.IndexOf(placeholder);
                VMLocator.Main.Icons.Remove(placeholder);
                VMLocator.Main.Icons.Insert(index, icon);
                icon.Pinned = true;
            }
            else
            {
                VMLocator.Main.Icons.Remove(placeholder);
                if (icon.IsActive)
                    VMLocator.Main.Icons.Add(icon);
            }
            // Clean up events and variables.
            isMouseDown = false;
            WindowManager.Manager.CursorPositionChanged -= Manager_CursorPositionChanged;
            itemsControl.ReleaseMouseCapture();
            itemsControl.MouseUp -= itemsControl_MouseUp;
            itemsControl.MouseMove -= itemsControl_MouseMove;
            draggedIconWindow.Close();
            draggedIconWindow = null;
            icon = null;
            placeholder = null;
        }

        private bool IsInDock(double x, double y)
        {
            return x >= 0 && x <= itemsControl.ActualWidth && y >= 0 && y <= itemsControl.ActualHeight;
        }

        void itemsControl_MouseMove(object sender, MouseEventArgs e)
        {
            mouseY = e.GetPosition(itemsControl).Y;
            mouseX = e.GetPosition(itemsControl).X;
            if (IsInDock(mouseX, mouseY))
            {
                var index = GetDropIndex(mouseX);
                if (placeholder == null)
                    placeholder = new IconViewModel();
                if (VMLocator.Main.Icons.IndexOf(placeholder) != index)
                {
                    VMLocator.Main.Icons.Remove(placeholder);
                    VMLocator.Main.Icons.Insert(index, placeholder);
                }
            }
            else
            {
                VMLocator.Main.Icons.Remove(placeholder);
                placeholder = null;
            }
        }

        public FrameworkElement FindFrameworkElement(IconViewModel icon)
        {
            return Panel.Children.OfType<FrameworkElement>().FirstOrDefault(c => c.DataContext == icon);
        }

        public int GetDropIndex(double x)
        {
            double cumul = 0;
            int index = 0;
            foreach (FrameworkElement item in Panel.Children)
            {
                var width = item.ActualWidth;
                if (x < cumul + width) // If mouse is before icon (before half width)
                    return index; // Set index after last icon
                cumul += width;
                index++;
            }
            return index - 1;
        }

        void Manager_CursorPositionChanged(object sender, Handlers.CursorPosEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var iconSize = VMLocator.Main.IconSize;
                draggedIconWindow.Left = e.CursorPosition.X - iconSize / 2;
                draggedIconWindow.Top = e.CursorPosition.Y - iconSize / 2;
                if (!draggedIconWindow.IsActive)
                    draggedIconWindow.Show();
            });
        }
    }
}
