using CleverDock.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Handlers
{
    public class ThemeEventArgs : EventArgs
    {
        public Theme Theme { get; set; }
        public ThemeEventArgs(Theme theme)
        {
            Theme = theme;
        }
    }
}
