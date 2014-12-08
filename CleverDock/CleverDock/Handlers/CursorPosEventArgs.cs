using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CleverDock.Handlers
{
    public class CursorPosEventArgs : EventArgs
    {
        public Point CursorPosition { get; set; }
        public CursorPosEventArgs(Point cursorPosition)
        {
            CursorPosition = cursorPosition;
        }
    }
}
