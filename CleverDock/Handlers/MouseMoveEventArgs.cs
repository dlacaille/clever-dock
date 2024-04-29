using System.Windows;

namespace CleverDock.Handlers;

public class MouseMoveEventArgs : EventArgs
{
    public MouseMoveEventArgs(Point cursorPosition)
    {
        CursorPosition = cursorPosition;
    }

    public Point CursorPosition { get; set; }
}