using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Model
{
    public class ThemeSettings
    {
        public static ThemeSettings DefaultThemeSettings = new ThemeSettings
        {
            IconPaddingX = 15,
            IconPaddingY = 20
        };

        public string ThemeName { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public int IconPaddingX { get; set; }
        public int IconPaddingY { get; set; }
    }
}
