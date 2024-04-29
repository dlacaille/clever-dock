using System.Timers;
using System.Windows;
using System.Windows.Interop;
using CleverDock.Helpers;
using CleverDock.Interop;
using CleverDock.Managers;
using Microsoft.Win32;
using Timer = System.Timers.Timer;

namespace CleverDock;

/// <summary>
///     Interaction logic for WidgetsWindow.xaml
/// </summary>
public partial class WidgetsWindow : Window
{
    public WidgetsWindow()
    {
        InitializeComponent();
        ShowInTaskbar = false;
        Loaded += WidgetsWindow_Loaded;
        WindowManager.Manager.ActiveWindowChanged += Manager_ActiveWindowChanged;
        SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

        timer = new Timer();
        timer.AutoReset = false;
        timer.Elapsed += timer_Elapsed;
        StartTimer();
        UpdateClock();
    }

    private Timer timer { get; }

    private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            SetDimensions();
            SetTopmost();
        });
    }

    public static void SetWindowExTransparent(IntPtr hwnd)
    {
        var extendedStyle = (int)WindowInterop.GetWindowLongPtr(hwnd, WindowInterop.GWL_EXSTYLE);
        WindowInterop.SetWindowLongPtr(hwnd, WindowInterop.GWL_EXSTYLE,
            extendedStyle | WindowInterop.WS_EX_TRANSPARENT);
    }

    private void WidgetsWindow_Loaded(object sender, RoutedEventArgs e)
    {
        SetDimensions();
    }

    private void SetDimensions()
    {
        var resolution = ScreenHelper.GetScreenResolution();
        SetWindowExTransparent(new WindowInteropHelper(this).Handle);
        Left = resolution.Width - ActualWidth;
        Top = resolution.Height - ActualHeight;
    }

    private void Manager_ActiveWindowChanged(object sender, EventArgs e)
    {
        // Set Topmost when the active window changes to keep topmost position.
        SetTopmost();
    }

    private void timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        UpdateClock();
        StartTimer();
    }

    private void UpdateClock()
    {
        Dispatcher.Invoke(() => { Clock.Content = string.Format("{0:HH:mm}", DateTime.Now); });
    }

    private void StartTimer()
    {
        timer.Interval = GetInterval();
        timer.Start();
    }

    private double GetInterval()
    {
        var now = DateTime.Now;
        return (60 - now.Second) * 1000 - now.Millisecond;
    }

    public void SetTopmost()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            // Uses interop to place the window at topmost position.
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowInterop.SetWindowPos(hwnd, WindowInterop.HWND_BOTTOM, 0, 0, 0, 0,
                WindowInterop.SWP_NOMOVE | WindowInterop.SWP_NOSIZE);
        });
    }
}