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
            ShowInTaskbar = false;
            ThemeManager.Manager.ThemeWindow(this);
            Console.WriteLine("Render Capability is Tier " + (RenderCapability.Tier >> 16));
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),
               new FrameworkPropertyMetadata { DefaultValue = 30 });
            dockHideTimer = new Timer(SettingsManager.Settings.DockHideDelay);
            dockHideTimer.Elapsed += dockHideTimer_Elapsed;
            dockShowTimer = new Timer(SettingsManager.Settings.DockShowDelay);
            dockShowTimer.Elapsed += dockShowTimer_Elapsed;
        }

        void Manager_CursorPositionChanged(object sender, Handlers.CursorPosEventArgs e)
        {
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
                bool hover = DockIcons.IsMouseOver;
                bool isInHotspot = MouseHotspot.Contains(WindowManager.Manager.CursorPosition);
                bool isActiveWindowOverlapping = WindowManager.Manager.ActiveWindowRect.IntersectsWith(Rect);
                return DockIsVisible && !isInHotspot && !hover && isActiveWindowOverlapping;
            }
        }

        Rect MouseHotspot
        {
            get { return new Rect(DockLeft, ScreenHeight - HotspotHeight, DockIcons.ActualWidth, HotspotHeight); }
        }

        void Manager_ActiveWindowRectChanged(object sender, Handlers.WindowRectEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Rect rect = e.Rect; // Implicit conversion to Windows.Rect
                bool intersects = rect.IntersectsWith(Rect); // Check if the active window is intersecting with the dock.
                bool hover = DockIcons.IsMouseOver;
                if (intersects && !hover)
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