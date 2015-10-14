using System;
using System.Windows;

namespace CleverDock.Handlers
{
    public class MouseMoveEventArgs : EventArgs
    {
        public Point CursorPosition { get; set; }
        public MouseMoveEventArgs(Point cursorPosition)
        {
            CursorPosition = cursorPosition;
        }
    }
}
