using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using CleverDock.Managers;
using CleverDock.Interop;

namespace CleverDock
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double Distance = 20;

        public MainWindow()
        {
            InitializeComponent();
            SetDimensions();
            SourceInitialized += MainWindow_SourceInitialized;
            Loaded += MainWindow_Loaded;
            DockIcons.SizeChanged += DockIcons_SizeChanged;
            DockIcons.LoadSettings();
            WindowManager.Manager.ActiveWindowChanged += Manager_ActiveWindowChanged;
            Application.Current.Exit += Application_Exit;
            ShowInTaskbar = false;
            LoadTheme("Metal");
        }

        void Manager_ActiveWindowChanged(object sender, EventArgs e)
        {
            SetTopmost();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetTopmost();
        }

        void LoadTheme(string name)
        {
            var theme = Application.LoadComponent(new Uri("/Cleverdock;component/Themes/" + name + ".xaml", UriKind.Relative)) as ResourceDictionary;
            Resources.MergedDictionaries.Add(theme);
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

        public void SetDimensions()
        {
            WorkAreaManager.SetWorkingArea(0, 0, 1920, 1080);
            WindowState = System.Windows.WindowState.Maximized;
            Width = 1920;
            Height = 1080;
            DockIcons.Height = SettingsManager.Settings.OuterIconSize;
            DockPanelBackground.Height = DockPanelStroke.Height = SettingsManager.Settings.OuterIconSize + 4;
            PlaceDock();
        }

        public void PlaceDock()
        {
            DockIcons.SetValue(Canvas.TopProperty, Math.Round(Height - DockIcons.ActualHeight - Distance));
            DockIcons.SetValue(Canvas.LeftProperty, Math.Round(Width/2 - DockIcons.ActualWidth/2));
        }
    }
}