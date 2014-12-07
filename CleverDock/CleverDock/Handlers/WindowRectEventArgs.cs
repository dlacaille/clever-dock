using CleverDock.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CleverDock.Handlers
{
    public class WindowRectEventArgs : EventArgs
    {
        public Rect Rect { get; set; }
        public WindowRectEventArgs(Rect rect)
        {
            Rect = rect;
        }
    }
}
