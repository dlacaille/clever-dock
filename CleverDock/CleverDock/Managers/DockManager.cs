using CleverDock.Helpers;
using CleverDock.Interop;
using CleverDock.Patterns;
using CleverDock.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace CleverDock.Managers
{
    public class DockManager
    {
        public Window Window;

        private static DockManager manager;
        public static DockManager Manager
        {
            // Singleton Pattern
            get { return manager ?? (manager = new DockManager()); }
        }

        public DockManager()
        {
            WindowLoadedCommand = new ActionCommand<Window>(WindowLoadedAction);
            LeftMouseDownCommand = new ActionCommand<IconViewModel>(LeftMouseDownAction);
            LeftMouseUpCommand = new ActionCommand<IconViewModel>(LeftMouseUpAction);
        }

        public ActionCommand<Window> WindowLoadedCommand { get; private set; }
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
            HwndSource source = PresentationSource.FromVisual(Window) as HwndSource;
            source.AddHook(WndProc);
            // These two lines are necessary to capture the HSHELL_FLASH events in the above hook.
            WindowInterop.RegisterShellHookWindow(handle);
            WindowInterop.RegisterWindowMessage("SHELLHOOK");
            SetWindowPosition();
        }

        public void SetWorkingArea(bool reserveScrenSpace)
        {
            var reserved = reserveScrenSpace ? (int)Window.Height : 0;
            WorkAreaManager.SetWorkingArea(0, 0, ScreenHelper.ScreenWidth, ScreenHelper.ScreenHeight - reserved);
        }

        public ActionCommand<IconViewModel> LeftMouseDownCommand { get; private set; }
        private void LeftMouseDownAction(IconViewModel icon)
        {
        }

        public ActionCommand<IconViewModel> LeftMouseUpCommand { get; private set; }
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
            {
                // Find the window with corresponding Hwnd and bounce it.
                foreach (IconViewModel icon in VMLocator.Main.Icons)
                    if (icon.Windows.Any(w => w.Hwnd == lParam))
                        icon.AnimateIconBounce();
            }
            return IntPtr.Zero;
        }

        private void SetWindowPosition()
        {
            Window.Left = (ScreenHelper.ScreenWidth - Window.Width) / 2;
            Window.Top = ScreenHelper.ScreenHeight - Window.Height;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SetWindowPosition();
            });
        }

        private void Manager_ActiveWindowChanged(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SetTopmost();
            });
        }

        private void SetTopmost()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Uses interop to place the window at topmost position.
                var hwnd = new WindowInteropHelper(Window).Handle;
                WindowInterop.SetWindowPos(hwnd, WindowInterop.HWND_TOPMOST, 0, 0, 0, 0, WindowInterop.SWP_NOMOVE | WindowInterop.SWP_NOSIZE);
            });
        }
    }
}
