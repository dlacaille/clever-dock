using CleverDock.Graphics;
using CleverDock.Handlers;
using CleverDock.Model;
using CleverDock.Views.Decorators;
using SharpDX;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace CleverDock.Views
{
    public class DockIconDragDecorator
    {
        private Dock dock { get; set; }
        private Point mousePositionStart;
        private DockIcon draggedIcon;
        private bool dragStarted;
        private int dragStartIndex;

        public DockIconDragDecorator(Dock dock)
        {
            this.dock = dock;
            dock.Subviews.Added += Subviews_Added;
        }

        void Subviews_Added(object sender, ViewEventArgs e)
        {
            if (!(e.View is DockIcon))
                return;
            var icon = (DockIcon)e.View;
            // Unsubscribe to the events in case they are already.
            icon.MouseLeftButtonDown -= icon_MouseLeftButtonDown;
            icon.MouseLeftButtonUp -= icon_MouseLeftButtonUp;
            icon.MouseMove -= icon_MouseMove;
            // Subscribe to the events.
            icon.MouseLeftButtonDown += icon_MouseLeftButtonDown;
            icon.MouseLeftButtonUp += icon_MouseLeftButtonUp;
            icon.MouseMove += icon_MouseMove;
        }

        void icon_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var icon = (DockIcon)sender;
            if (draggedIcon == null || icon != draggedIcon)
                return;
            // If the icon is being dragged.
            if (!dragStarted)
            {
                Console.WriteLine("Mouse not captured");
                // Remember the icon's index in the dock.
                dragStartIndex = dock.Subviews.IndexOf(draggedIcon);
                // Remove it from the dock.
                draggedIcon.RemoveFromSuperview();
                // And add it to the scene.
                var view = dock.Scene.View;
                view.Subviews.Add(draggedIcon);
                // Capture the mouse.
                draggedIcon.CaptureMouse();
                dragStarted = true;
            }
            // Change the dragged icon's position
            var mousePos = e.MouseDevice.GetPosition(icon.Scene.Window);
            var width = draggedIcon.Bounds.Width;
            var height = draggedIcon.Bounds.Height;
            draggedIcon.Bounds = new RectangleF((float)mousePos.X - width/2, (float)mousePos.Y - height/2, width, height);
        }

        void icon_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var icon = draggedIcon = (DockIcon)sender;
            mousePositionStart = e.MouseDevice.GetPosition(icon.Scene.Window);
        }

        void icon_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var icon = (DockIcon)sender;
            if (draggedIcon == null || icon != draggedIcon)
                return;
            // Stop capturing the mouse.
            if (icon.IsMouseCaptured)
                icon.Scene.CapturedMouseView = null;
            // If the icon has a window, add it back to the dock.
            if (icon.Windows.Any())
            {
                icon.RemoveFromSuperview();
            }
            // Remove dragged state.
            draggedIcon = null;
            dragStarted = false;
        }
    }
}
