using System;
using CleverDock.Interop;

namespace CleverDock.Handlers
{
    public class WindowEventArgs : EventArgs
    {
        public Win32Window Window { get; set; }
        public WindowEventArgs(Win32Window window)
        {
            Window = window;
        }
    }
}
