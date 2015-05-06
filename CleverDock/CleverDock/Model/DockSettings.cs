using CleverDock.Managers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CleverDock.Model
{
    public class DockSettings : INotifyPropertyChanged
    {
        public const string SETTINGS_VERSION = "0.4.1";

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public static DockSettings DefaultDockSettings = new DockSettings
        {
            IconSize = 48,
            IconPaddingX = 15,
            IconPaddingY = 20,
            CollapseDuration = 0.2,
            HotspotHeight = 20,
            DockEdgeSpacing = 20,
            DockHideDuration = 0.5,
            DockShowDuration = 0.3,
            DockHideDelay = 0,
            DockShowDelay = 0,
            AutoHide = true,
            SaveAutomatically = true,
            RemoveTaskbar = true,
            ReserveScreenSpace = false,
            Icons = new List<SerializableIconInfo>(),
            Theme = ThemeManager.DefaultTheme
        };

        public string Version = SETTINGS_VERSION;

        private double _collapseDuration;
        public double CollapseDuration
        {
            get { return _collapseDuration; }
            set { _collapseDuration = value; OnPropertyChanged(); }
        }

        private double _dockHideDuration;
        public double DockHideDuration
        {
            get { return _dockHideDuration; }
            set { _dockHideDuration = value; OnPropertyChanged(); }
        }
        private double _dockShowDuration;
        public double DockShowDuration
        {
            get { return _dockShowDuration; }
            set { _dockShowDuration = value; OnPropertyChanged(); }
        }
        private double _dockHideDelay;
        public double DockHideDelay
        {
            get { return _dockHideDelay; }
            set { _dockHideDelay = value; OnPropertyChanged(); }
        }
        private double _dockShowDelay;
        public double DockShowDelay
        {
            get { return _dockShowDelay; }
            set { _dockShowDelay = value; OnPropertyChanged(); }
        }

        private int _iconPaddingX;
        public int IconPaddingX
        {
            get { return _iconPaddingX; }
            set { _iconPaddingX = value; OnPropertyChanged(); }
        }

        private int _iconPaddingY;
        public int IconPaddingY
        {
            get { return _iconPaddingY; }
            set { _iconPaddingY = value; OnPropertyChanged(); }
        }

        private int _hotspotHeight;
        public int HotspotHeight
        {
            get { return _hotspotHeight; }
            set { _hotspotHeight = value; OnPropertyChanged(); }
        }

        private int _dockEdgeSpacing;
        public int DockEdgeSpacing
        {
            get { return _dockEdgeSpacing; }
            set { _dockEdgeSpacing = value; OnPropertyChanged(); }
        }

        private int _iconSize;
        public int IconSize
        {
            get { return _iconSize; }
            set { _iconSize = value; OnPropertyChanged(); }
        }

        private Theme _theme;
        public Theme Theme
        {
            get { return _theme; }
            set { _theme = value; OnPropertyChanged(); }
        }

        private List<SerializableIconInfo> _icons;
        public List<SerializableIconInfo> Icons
        {
            get { return _icons; }
            set { _icons = value; OnPropertyChanged(); }
        }

        private bool _saveAutomatically;
        public bool SaveAutomatically
        {
            get { return _saveAutomatically; }
            set { _saveAutomatically = value; OnPropertyChanged(); }
        }

        private bool _reserveScreenSpace;
        public bool ReserveScreenSpace
        {
            get { return _reserveScreenSpace; }
            set {  _reserveScreenSpace = value; OnPropertyChanged(); }
        }

        private bool _removeTaskbar;
        public bool RemoveTaskbar
        {
            get { return _removeTaskbar; }
            set { _removeTaskbar = value; OnPropertyChanged(); }
        }

        private bool _autoHide;
        public bool AutoHide
        {
            get { return _autoHide; }
            set { _autoHide = value; OnPropertyChanged(); }
        }

        public int OuterIconWidth
        {
            get { return IconSize + IconPaddingX; }
        }

        public int OuterIconHeight
        {
            get { return IconSize + IconPaddingY; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}