using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Helpers
{
    public static class ScreenHelper
    {
        public static int ScreenWidth
        {
            get { return (int)System.Windows.SystemParameters.PrimaryScreenWidth; }
        }

        public static int ScreenHeight
        {
            get { return (int)System.Windows.SystemParameters.PrimaryScreenHeight; }
        }
    }
}
