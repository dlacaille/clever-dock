using CleverDock.ViewModels;
using Funq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Patterns
{
    public class VMLocator
    {
        private static readonly Container container = new Container();

        public VMLocator()
        {
            container.Register(c => new MainViewModel());
            container.Register(c => new ThemeSettingsViewModel());

        }

        public static MainViewModel Main
        {
            get { return container.Resolve<MainViewModel>(); }
        }

        public static ThemeSettingsViewModel ThemeSettings
        {
            get { return container.Resolve<ThemeSettingsViewModel>(); }
        }
    }
}
