using CleverDock.Managers;
using CleverDock.Interop;
using CleverDock.Patterns;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CleverDock.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Interop;
using CleverDock.Helpers;
using System.Windows.Media.Imaging;
using CleverDock.Tools;

namespace CleverDock.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private SettingsModel settings = new SettingsModel();

        public MainViewModel()
        {
            Application.Current.Exit += Current_Exit;
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            settings.SaveAsFile(SettingsModel.SETTINGS_FILE);
        }

        #region Commands

        #endregion

        #region Properties

        /// <summary>
        /// Duration time in seconds for the collapsing animation.
        /// </summary>
        public double CollapseDuration
        {
            get { return settings.CollapseDuration; }
            set { if (value != settings.CollapseDuration) { settings.CollapseDuration = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Duration time in seconds for the hiding animation.
        /// </summary>
        public double DockHideDuration
        {
            get { return settings.DockHideDuration; }
            set { if (value != settings.DockHideDuration) { settings.DockHideDuration = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Duration time in seconds for the show animation.
        /// </summary>
        public double DockShowDuration
        {
            get { return settings.DockShowDuration; }
            set { if (value != settings.DockShowDuration) { settings.DockShowDuration = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Seconds the dock should stay visible before hiding.
        /// </summary>
        public double DockHideDelay
        {
            get { return settings.DockHideDelay; }
            set { if (value != settings.DockHideDelay) { settings.DockHideDelay = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Seconds the mouse must stay in the hotspot before showing the dock.
        /// </summary>
        public double DockShowDelay
        {
            get { return settings.DockShowDelay; }
            set { if (value != settings.DockShowDelay) { settings.DockShowDelay = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Height of the hotspot at the bottom of the screen.
        /// </summary>
        public int HotspotHeight
        {
            get { return settings.HotspotHeight; }
            set { if (value != settings.HotspotHeight) { settings.HotspotHeight = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Size of the icons in pixels.
        /// </summary>
        public int IconSize
        {
            get { return settings.IconSize; }
            set { if (value != settings.IconSize) { settings.IconSize = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Theme of the dock.
        /// </summary>
        public ThemeModel Theme
        {
            get { return settings.Theme; }
            set { if (value != settings.Theme) { settings.Theme = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<IconViewModel> _icons;
        /// <summary>
        /// Icons of the dock.
        /// </summary>
        public ObservableCollection<IconViewModel> Icons
        {
            get
            {
                if (IsInDesignMode)
                {
                    var icon =  new BitmapImage(new Uri("pack://application:,,,/CleverDock;component/Content/Chrome.png"));
                    var blurred = BitmapEffectHelper.GaussianBlur(icon, 2.5f);
                    _icons = new ObservableCollection<IconViewModel> {
                        new IconViewModel(new IconModel { Name = "chrome.exe", Path = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" }),
                        new IconViewModel(new IconModel { Name = "sublime_text.exe", Path = @"C:\Program Files\Sublime Text 3\sublime_text.exe" }),
                    };
                }
                if (_icons == null)
                    _icons = new ObservableCollection<IconViewModel>(settings.Icons.Select(i => new IconViewModel(i)));
                return _icons;
            }
        }

        /// <summary>
        /// Save the settings automatically or when the dock is closed.
        /// </summary>
        public bool SaveAutomatically
        {
            get { return settings.SaveAutomatically; }
            set { if (value != settings.SaveAutomatically) { settings.SaveAutomatically = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Reserve the bottom of the screen for the dock.
        /// </summary>
        public bool ReserveScreenSpace
        {
            get { return settings.ReserveScreenSpace; }
            set 
            { 
                if (value != settings.ReserveScreenSpace) 
                { 
                    settings.ReserveScreenSpace = value; 
                    OnPropertyChanged(); 
                }
                DockManager.Manager.SetWorkingArea(settings.ReserveScreenSpace);
            }
        }

        /// <summary>
        /// Remote windows taskbar to replace it with the dock.
        /// </summary>
        public bool RemoveTaskbar
        {
            get { return settings.RemoveTaskbar; }
            set 
            { 
                if (value != settings.RemoveTaskbar) 
                {
                    settings.RemoveTaskbar = value; 
                    OnPropertyChanged(); 
                }
                TaskbarManager.SetTaskbarVisibility(!value);
            }
        }

        /// <summary>
        /// Hide the dock automatically when it occludes an other window.
        /// </summary>
        public bool AutoHide
        {
            get { return settings.AutoHide; }
            set { if (value != settings.AutoHide) { settings.AutoHide = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Show the widgets in the corner of the screen.
        /// </summary>
        public bool ShowWidgets
        {
            get { return settings.ShowWidgets; }
            set { if (value != settings.ShowWidgets) { settings.ShowWidgets = value; OnPropertyChanged(); } }
        }

        #endregion
    }
}