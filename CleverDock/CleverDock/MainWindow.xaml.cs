using CleverDock.Interop;
using CleverDock.Managers;
using CleverDock.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CleverDock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainScene scene = new MainScene();

        public MainWindow()
        {
            SetDimensions();
            InitializeComponent();
            this.Direct2DControl.Scene = this.scene;
            WindowManager.Manager.ActiveWindowChanged += Manager_ActiveWindowChanged;
            WindowManager.Manager.Start();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            WindowManager.Manager.Stop();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetTopmost();
        }

        void Manager_ActiveWindowChanged(object sender, EventArgs e)
        {
            SetTopmost();
        }

        public int ScreenWidth
        {
            get { return (int)System.Windows.SystemParameters.PrimaryScreenWidth; }
        }

        public int ScreenHeight
        {
            get { return (int)System.Windows.SystemParameters.PrimaryScreenHeight; }
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
            WindowState = System.Windows.WindowState.Maximized;
            Width = ScreenWidth;
            Height = ScreenHeight;
        }
    }
}
