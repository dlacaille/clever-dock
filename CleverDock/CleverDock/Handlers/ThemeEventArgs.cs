using CleverDock.Interop;
using CleverDock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Handlers
{
    public class ThemeEventArgs : EventArgs
    {
        public ThemeModel Theme { get; set; }
        public ThemeEventArgs(ThemeModel theme)
        {
            Theme = theme;
        }
    }
}
