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
            Window = this;
            InitializeComponent();
            SourceInitialized += MainWindow_SourceInitialized;
            Loaded += MainWindow_Loaded;
            DockIcons.SizeChanged += DockIcons_SizeChanged;
            DockIcons.LoadSettings();
            WindowManager.Manager.ActiveWindowChanged += Manager_ActiveWindowChanged;
            Application.Current.Exit += Application_Exit;
            ShowInTaskbar = false;
            new AutoHideDecorator(this);
            ThemeManager.Manager.ThemeWindow(this);
            Console.WriteLine("Render Capability is Tier " + (RenderCapability.Tier >> 16));
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),
               new FrameworkPropertyMetadata { DefaultValue = 60 });
        }
        
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            Console.WriteLine(wParam);
            if (wParam.ToInt32() == WindowInterop.HSHELL_FLASH)
            {
                foreach (DockIcon icon in DockIcons.Children)
                    if (icon.Windows.Any(w => w.Hwnd == lParam))
                        icon.AnimateIconBounce();
            }
            return IntPtr.Zero;
        }

        void Manager_ActiveWindowChanged(object sender, EventArgs e)
        {
            SetTopmost();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetTopmost();
            HwndSource source = PresentationSource.FromVisual(MainWindow.Window) as HwndSource;
            source.AddHook(MainWindow.Window.WndProc);
            WindowInterop.RegisterShellHookWindow(new WindowInteropHelper(this).Handle);
            var msg = (int)WindowInterop.RegisterWindowMessage("SHELLHOOK");
        }

        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
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