using System;
using CleverDock.Model;

namespace CleverDock.Handlers
{
    public class WindowEventArgs : EventArgs
    {
        public Window Window { get; set; }
        public WindowEventArgs(Window window)
        {
            Window = window;
        }
    }
}
