using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using CleverDock.Helpers;
using CleverDock.Interop;
using CleverDock.Patterns;
using CleverDock.ViewModels;
using Microsoft.Win32;

namespace CleverDock.Managers;

public class DockManager
{
    private static DockManager manager;
    public Window Window;

    public DockManager()
    {
        WindowLoadedCommand = new ActionCommand<Window>(WindowLoadedAction);
        LeftMouseDownCommand = new ActionCommand<IconViewModel>(LeftMouseDownAction);
        LeftMouseUpCommand = new ActionCommand<IconViewModel>(LeftMouseUpAction);
    }

    public WidgetsWindow WidgetsWindow { get; set; }

    public static DockManager Manager =>
        // Singleton Pattern
        manager ?? (manager = new DockManager());

    public ActionCommand<Window> WindowLoadedCommand { get; private set; }

    public ActionCommand<IconViewModel> LeftMouseDownCommand { get; private set; }

    public ActionCommand<IconViewModel> LeftMouseUpCommand { get; private set; }

    private void WindowLoadedAction(Window _window)
    {
        Window = _window;
        var handle = new WindowInteropHelper(Window).Handle;
        WindowManager.Manager.SetDockHwnd(handle);
        WindowManager.Manager.Start();
        ThemeManager.Manager.ThemeWindow();
        if (VMLocator.Main.RemoveTaskbar)
            TaskbarManager.SetTaskbarVisibility(false);
        SetWorkingArea(VMLocator.Main.ReserveScreenSpace);
        // Subscribe to window events.
        Application.Current.Exit += Current_Exit;
        WindowManager.Manager.ActiveWindowChanged += Manager_ActiveWindowChanged;
        SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        // Add a hook to receive window events from Windows.
        var source = PresentationSource.FromVisual(Window) as HwndSource;
        source.AddHook(WndProc);
        // These two lines are necessary to capture the window events in the above hook.
        WindowInterop.RegisterShellHookWindow(handle);
        WindowInterop.RegisterWindowMessage("SHELLHOOK");
        SetWindowPosition();
        // Show the widgets window
        ShowWidgets();
    }

    public void ShowWidgets()
    {
        if (WidgetsWindow == null)
            WidgetsWindow = new WidgetsWindow();
        WidgetsWindow.Show();
    }

    public void SetWorkingArea(bool reserveScrenSpace)
    {
        var reserved = reserveScrenSpace ? 100 : 0;
        var resolution = ScreenHelper.GetScreenResolution(true);
        WorkAreaManager.SetWorkingArea(0, 0, (int)resolution.Width, (int)resolution.Height - reserved);
    }

    private void LeftMouseDownAction(IconViewModel icon)
    {
    }

    private void LeftMouseUpAction(IconViewModel icon)
    {
        icon.Run();
    }

    private void Current_Exit(object sender, ExitEventArgs e)
    {
        if (VMLocator.Main.RemoveTaskbar)
            TaskbarManager.SetTaskbarVisibility(true);
        WindowManager.Manager.Stop();
    }

    public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        // If windows sends the HSHELL_FLASH event, a window in the taskbar is flashing.
        if (wParam.ToInt32() == WindowInterop.HSHELL_FLASH)
            // Find the window with corresponding Hwnd and bounce it.
            FindIcon(lParam)?.AnimateIconBounce();
        // If windows sends the HSHELL_GETMINRECT event, a window in the taskbar is minimizing or maximizing.
        if (wParam.ToInt32() == WindowInterop.HSHELL_GETMINRECT)
        {
            var param = Marshal.PtrToStructure<WindowInterop.MinRectParam>(lParam);
            var icon = FindIcon(param.hWnd);
            if (icon == null)
                return IntPtr.Zero;
            var point = icon.Element.TransformToVisual(Window).Transform(new Point(0, 0));
            var rect = new WindowInterop.SRect
            {
                Bottom = (short)(point.Y + Window.Top + icon.Element.ActualHeight),
                Left = (short)(point.X + Window.Left),
                Right = (short)(point.X + Window.Left + icon.Element.ActualWidth),
                Top = (short)(point.Y + Window.Top)
            };
            var newParam = new WindowInterop.MinRectParam
            {
                hWnd = param.hWnd,
                Rect = rect
            };
            Marshal.StructureToPtr(newParam, lParam, true);
            handled = true;
            return new IntPtr(1);
        }

        return IntPtr.Zero;
    }

    public IconViewModel FindIcon(IntPtr hwnd)
    {
        return VMLocator.Main.Icons.FirstOrDefault(i => i.Windows.Any(w => w.Hwnd == hwnd));
    }

    private void SetWindowPosition()
    {
        var resolution = ScreenHelper.GetScreenResolution();
        Window.Left = (resolution.Width - Window.Width) / 2;
        Window.Top = resolution.Height - Window.Height;
    }

    private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() => { SetWindowPosition(); });
    }

    private void Manager_ActiveWindowChanged(object sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() => { SetTopmost(); });
    }

    private void SetTopmost()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            // Uses interop to place the window at topmost position.
            var hwnd = new WindowInteropHelper(Window).Handle;
            WindowInterop.SetWindowPos(hwnd, WindowInterop.HWND_TOPMOST, 0, 0, 0, 0,
                WindowInterop.SWP_NOMOVE | WindowInterop.SWP_NOSIZE);
        });
    }
}