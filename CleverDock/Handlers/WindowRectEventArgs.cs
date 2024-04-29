using System.Windows;

namespace CleverDock.Handlers;

public class WindowRectEventArgs : EventArgs
{
    public WindowRectEventArgs(Rect rect)
    {
        Rect = rect;
    }

    public Rect Rect { get; set; }
}