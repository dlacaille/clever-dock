using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using CleverDock.Managers;
using CleverDock.Interop;
using System.IO;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media;
using CleverDock.Tools;
using System.Timers;

namespace CleverDock
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Window;

        public double Distance = 0;
        public double TopMargin = 20;
        public double HotspotHeight = 20;
        public bool DockIsVisible = true;

        private Timer dockShowTimer = null;
        private Timer dockHideTimer = null;

        public MainWindow()
        {
            Window = this;
            InitializeComponent();
            SetDimensions();
            SourceInitialized += MainWindow_SourceInitialized;
            Loaded += MainWindow_Loaded;
            DockIcons.SizeChanged += DockIcons_SizeChanged;
            DockIcons.LoadSettings();
            WindowManager.Manager.ActiveWindowChanged += Manager_ActiveWindowChanged;
            WindowManager.Manager.ActiveWindowRectChanged += Manager_ActiveWindowRectChanged;
            WindowManager.Manager.CursorPositionChanged += Manager_CursorPositionChanged;
            Application.Current.Exit += Application_Exit;
            SettingsManager.Settings.PropertyChanged += Settings_PropertyChanged;
            ShowInTaskbar = false;
            ThemeManager.Manager.ThemeWindow(this);
            Console.WriteLine("Render Capability is Tier " + (RenderCapability.Tier >> 16));
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),
               new FrameworkPropertyMetadata { DefaultValue = 30 });
            dockHideTimer = new Timer(SettingsManager.Settings.DockHideDelay);
            dockHideTimer.Elapsed += dockHideTimer_Elapsed;
            dockHideTimer.AutoReset = false;
            dockShowTimer = new Timer(SettingsManager.Settings.DockShowDelay);
            dockShowTimer.Elapsed += dockShowTimer_Elapsed;
            dockShowTimer.AutoReset = false;
        }

        void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AutoHide")
            {
                if (!SettingsManager.Settings.AutoHide && !DockIsVisible)
                    ShowDock();
            }

        }

        void Manager_CursorPositionChanged(object sender, Handlers.CursorPosEventArgs e)
        {
            if (!SettingsManager.Settings.AutoHide)
                return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!DockIsVisible && !ShouldShowDock)
                {
                    dockShowTimer.Stop();
                    dockShowTimer.Start();
                }
                else if (DockIsVisible && !ShouldHideDock)
                {
                    dockHideTimer.Stop();
                    dockHideTimer.Start();
                }
            });
        }

        void dockHideTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ShouldHideDock)
                    HideDock();
            });
        }

        void dockShowTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ShouldShowDock)
                    ShowDock();
            });
        }

        bool ShouldShowDock
        {
            get
            {
                if (DockIsVisible)
                    return false;
                bool hover = DockIcons.IsMouseOver;
                bool isInHotspot = MouseHotspot.Contains(WindowManager.Manager.CursorPosition);
                return !DockIsVisible && isInHotspot;
            }
        }

        bool ShouldHideDock
        {
            get
            {
                if (!DockIsVisible)
                    return false;
                var window = new Model.Window(WindowManager.Manager.ActiveWindow);
                if (window.FileName.EndsWith("explorer.exe") && window.Title == "")
                    return false; // Explorer.exe with no title should be the Desktop in most cases.
                return WindowManager.Manager.ActiveWindowRect.IntersectsWith(Rect) // Dock intersects with foreground window
                    && DockIsVisible
                    && !MouseHotspot.Contains(WindowManager.Manager.CursorPosition) // Mouse is not in hotspot 
                    && !DockIcons.IsMouseOver; // Mouse is not over the dock icons.
            }
        }

        Rect MouseHotspot
        {
            get 
            {
                double hotspotWidth = Math.Max(DockIcons.ActualWidth, ScreenWidth / 2);
                double hotspotLeft = (ScreenWidth - hotspotWidth) / 2;
                return new Rect(hotspotLeft, ScreenHeight - HotspotHeight, hotspotWidth, HotspotHeight); 
            }
        }

        void Manager_ActiveWindowRectChanged(object sender, Handlers.WindowRectEventArgs e)
        {
            if (!SettingsManager.Settings.AutoHide)
                return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (ShouldHideDock)
                    HideDock();
                else
                    ShowDock();
            });
        }

        void Manager_ActiveWindowChanged(object sender, EventArgs e)
        {
            SetTopmost();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetTopmost();
        }

        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            WindowManager.Manager.SetDockHwnd(new WindowInteropHelper(this).Handle); 
        }

        public Point Dimensions
        {
            get { return new Point(Width, Height); }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            DockIcons.SaveSettings();
        }

        private void DockIcons_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PlaceDock();
        }

        public void SetTopmost()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var hwnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                WindowInterop.SetWindowPos(hwnd, WindowInterop.HWND_TOPMOST, 0, 0, 0, 0, WindowInterop.SWP_NOMOVE | WindowInterop.SWP_NOSIZE);
            });
        }

        public void HideDock()
        {
            if (!DockIsVisible)
                return;
            double dockHeight = DockIcons.ActualHeight + Distance + TopMargin;
            AnimationTools.TranslateY(SettingsManager.Settings.DockHideDuration, DockTop + dockHeight, Canvas.TopProperty, DockIcons);
            DockIsVisible = false;
        }

        public void ShowDock()
        {
            if (DockIsVisible)
                return;
            AnimationTools.TranslateY(SettingsManager.Settings.DockShowDuration, DockTop, Canvas.TopProperty, DockIcons);
            DockIsVisible = true;
        }

        public int ScreenWidth
        {
            get { return (int)System.Windows.SystemParameters.PrimaryScreenWidth; }
        }

        public int ScreenHeight
        {
            get { return (int)System.Windows.SystemParameters.PrimaryScreenHeight; }
        }

        public void SetDimensions()
        {
            WindowState = System.Windows.WindowState.Maximized;
            Width = ScreenWidth;
            Height = ScreenHeight;
            DockIcons.Height = SettingsManager.Settings.OuterIconSize;
            DockPanelBackground.Height = DockPanelStroke.Height = SettingsManager.Settings.OuterIconSize + 4;
            int reservedSpace = (int)(SettingsManager.Settings.ReserveScreenSpace ? DockPanelBackground.Height + Distance + TopMargin : 0);
            WorkAreaManager.SetWorkingArea(0, 0, ScreenWidth, ScreenHeight - reservedSpace);
            PlaceDock();
        }

        public double DockLeft
        {
            get { return Math.Round(ScreenWidth / 2 - DockIcons.ActualWidth / 2); }
        }

        public double DockTop
        {
            get { return Math.Round(ScreenHeight - DockIcons.Height - Distance); }
        }

        public Rect Rect
        {
            get { return new Rect(DockLeft, DockTop, DockIcons.ActualWidth, DockIcons.Height); }
        }

        public void PlaceDock()
        {
            DockIcons.SetValue(Canvas.TopProperty, DockTop);
            DockIcons.SetValue(Canvas.LeftProperty, DockLeft);
        }
    }
}