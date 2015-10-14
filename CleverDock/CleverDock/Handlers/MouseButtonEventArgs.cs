using System;
using System.Windows;

namespace CleverDock.Handlers
{
    public enum MouseButton
    {
        Left,
        Right
    }

    public class MouseButtonEventArgs : EventArgs
    {
        public Point CursorPosition { get; set; }
        public MouseButton MouseButton { get; set; }
        public MouseButtonEventArgs(MouseButton mouseButton, Point cursorPosition)
        {
            CursorPosition = cursorPosition;
            MouseButton = mouseButton;
        }
    }
}
