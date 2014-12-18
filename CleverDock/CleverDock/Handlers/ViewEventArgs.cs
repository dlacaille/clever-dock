using CleverDock.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Handlers
{
    public class ViewEventArgs : EventArgs
    {
        public View View { get; set; }

        public ViewEventArgs(View view)
        {
            View = view;
        }
    }
}
