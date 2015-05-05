using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Linq;
using CleverDock.Managers;
using CleverDock.Interop;
using System.IO;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media;
using CleverDock.Tools;
using System.Timers;
using CleverDock.Controls;
using CleverDock.Helpers;
using CleverDock.Decorators;

namespace CleverDock
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Window;

        public double TopPadding = 50;
        public double SidePadding = 200;
        public double Distance = 0;
        public bool ContextMenuOpened = false;

        public MainWindow()
        {
            InitializeComponent();
            // Do not show window in taskbar.
            ShowInTaskbar = false;
            // Expose the dock window to other classes.
            Window = this;
            // Register events.
            SourceInitialized += MainWindow_SourceInitialized;
            Loaded += MainWindow_Loaded;
            DockIcons.SizeChanged += DockIcons_SizeChanged;
            WindowManager.Manager.ActiveWindowChanged += Manager_ActiveWindowChanged;
            Application.Current.Exit += Application_Exit;
            // Decorate the window.
            new AutoHideDecorator(this);
            // Load settings.
            DockIcons.LoadSettings();
            // Load theme.
            ThemeManager.Manager.ThemeWindow(this);
            // Change framerate to 60fps.
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),
               new FrameworkPropertyMetadata { DefaultValue = 60 });
        }
        
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // If windows sends the HSHELL_FLASH event, a window in the taskbar is flashing.
            if (wParam.ToInt32() == WindowInterop.HSHELL_FLASH)
            {
                // Find the window with corresponding Hwnd and bounce it.
                foreach (DockIcon icon in DockIcons.Children)
                    if (icon.Windows.Any(w => w.Hwnd == lParam))
                        icon.AnimateIconBounce();
            }
            return IntPtr.Zero;
        }

        void Manager_ActiveWindowChanged(object sender, EventArgs e)
        {
            // Set Topmost when the active window changes to keep topmost position.
            SetTopmost();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Add a hook to receive window events from Windows.
            HwndSource source = PresentationSource.FromVisual(MainWindow.Window) as HwndSource;
            source.AddHook(MainWindow.Window.WndProc);
            // These two lines are necessary to capture the HSHELL_FLASH events in the above hook.
            WindowInterop.RegisterShellHookWindow(new WindowInteropHelper(this).Handle);
            WindowInterop.RegisterWindowMessage("SHELLHOOK");
        }

        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            // When the window has been initialized, set the dock dimensions setup the WindowManager.
            SetDimensions();
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
            SetDimensions();
        }

        public void SetTopmost()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Uses interop to place the window at topmost position.
                var hwnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                WindowInterop.SetWindowPos(hwnd, WindowInterop.HWND_TOPMOST, 0, 0, 0, 0, WindowInterop.SWP_NOMOVE | WindowInterop.SWP_NOSIZE);
            });
        }

        public void SetDimensions()
        {
            Width = Math.Min(DockIcons.ActualWidth + SidePadding * 2, ScreenHelper.ScreenWidth);
            Height = DockIcons.ActualHeight + Distance + TopPadding;
            Left = DockLeft;
            Top = DockTop;
            DockIcons.Height = SettingsManager.Settings.OuterIconHeight;
            DockPanelBackground.Height = DockPanelStroke.Height = SettingsManager.Settings.OuterIconHeight + 4;
            int reservedSpace = (int)(SettingsManager.Settings.ReserveScreenSpace ? DockPanelBackground.Height + Distance + TopPadding : 0);
            WorkAreaManager.SetWorkingArea(0, 0, ScreenHelper.ScreenWidth, ScreenHelper.ScreenHeight - reservedSpace);
            PlaceDock();
        }

        public double DockLeft
        {
            get { return Math.Round(ScreenHelper.ScreenWidth / 2 - DockIcons.ActualWidth / 2 - SidePadding); }
        }

        public double DockTop
        {
            get { return Math.Round(ScreenHelper.ScreenHeight - DockIcons.Height - Distance - TopPadding); }
        }

        public void PlaceDock()
        {
            DockIcons.SetValue(Canvas.TopProperty, TopPadding);
            DockIcons.SetValue(Canvas.LeftProperty, SidePadding);
        }
    }
}