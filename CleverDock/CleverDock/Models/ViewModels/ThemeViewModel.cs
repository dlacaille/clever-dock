using CleverDock.Managers;
using CleverDock.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CleverDock.Models.ViewModels
{
    public class ThemeViewModel : ViewModelBase
    {
        private ThemeModel model = new ThemeModel();

        public ThemeViewModel(ThemeModel _model)
        {
            model = _model;
            LoadCommand = new ActionCommand(LoadAction);
        }

        /// <summary>
        /// Name of the theme.
        /// </summary>
        public string Name { get { return model.Name; } set { if (value != model.Name) { model.Name = value; OnPropertyChanged(); } } }

        /// <summary>
        /// Path of the theme.
        /// </summary>
        public string Path { get { return model.Path; } set { if (value != model.Path) { model.Path = value; OnPropertyChanged(); } } }

        public ActionCommand LoadCommand { get; private set; }
        private void LoadAction()
        {
            ThemeManager.Manager.LoadTheme(model);
        }
    }
}
