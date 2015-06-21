using CleverDock.Helpers;
using CleverDock.Interop;
using CleverDock.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace CleverDock
{
    /// <summary>
    /// Interaction logic for WidgetsWindow.xaml
    /// </summary>
    public partial class WidgetsWindow : Window
    {
        private Timer timer { get; set; }

        public WidgetsWindow()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            Loaded += WidgetsWindow_Loaded;
            WindowManager.Manager.ActiveWindowChanged += Manager_ActiveWindowChanged;
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            timer = new System.Timers.Timer();
            timer.AutoReset = false;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            StartTimer();
            UpdateClock();
        }

        void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                SetDimensions();
            });
        }

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = (int)WindowInterop.GetWindowLongPtr(hwnd, WindowInterop.GWL_EXSTYLE);
            WindowInterop.SetWindowLongPtr(hwnd, WindowInterop.GWL_EXSTYLE, extendedStyle | WindowInterop.WS_EX_TRANSPARENT);
        }

        void WidgetsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetDimensions();
        }

        private void SetDimensions()
        {
            SetWindowExTransparent(new WindowInteropHelper(this).Handle);
            Left = ScreenHelper.ScreenWidth - ActualWidth;
            Top = ScreenHelper.ScreenHeight - ActualHeight;
        }

        void Manager_ActiveWindowChanged(object sender, EventArgs e)
        {
            // Set Topmost when the active window changes to keep topmost position.
            SetTopmost();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateClock();
            StartTimer();
        }

        private void UpdateClock()
        {
            Dispatcher.Invoke(() =>
            {
                Clock.Content = string.Format("{0:HH:mm}", DateTime.Now);
            });
        }

        private void StartTimer()
        {
            timer.Interval = GetInterval();
            timer.Start();
        }

        private double GetInterval()
        {
            DateTime now = DateTime.Now;
            return ((60 - now.Second) * 1000 - now.Millisecond);
        }

        public void SetTopmost()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Uses interop to place the window at topmost position.
                var hwnd = new WindowInteropHelper(this).Handle;
                WindowInterop.SetWindowPos(hwnd, WindowInterop.HWND_TOPMOST, 0, 0, 0, 0, WindowInterop.SWP_NOMOVE | WindowInterop.SWP_NOSIZE);
            });
        }
    }
}
