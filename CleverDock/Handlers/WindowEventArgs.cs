using CleverDock.Interop;

namespace CleverDock.Handlers;

public class WindowEventArgs : EventArgs
{
    public WindowEventArgs(Win32Window window)
    {
        Window = window;
    }

    public Win32Window Window { get; set; }
}