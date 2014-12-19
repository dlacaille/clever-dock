using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using CleverDock.Handlers;
using CleverDock.Interop;
using Window = CleverDock.Model.Window;
using System.Diagnostics;
using System.Text;

namespace CleverDock.Managers
{
    public class WindowManager
    {
        public List<Window> Windows { get; set; }

        private const long WS_OVERLAPPED = 0x00000000L;
        private const long WS_CHILD = 0x40000000L;
        private const long WS_POPUP = 0x80000000L;
        private const long WS_BORDER = 0x00800000L;
        private const long WS_VISIBLE = 0x10000000L;
        private const long TARGETWINDOW = WS_BORDER | WS_VISIBLE;
        private const long WS_EX_APPWINDOW = 0x00040000L;
        private const long WS_EX_TOOLWINDOW = 0x00000080L;
        private const long WS_EX_CLIENTEDGE = 0x00000200L;
        private const long WS_EX_WINDOWEDGE = 0x00000100L;
        private const long WS_EX_DLGMODALFRAME = 0x00000001L;
        private const long WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE;

        private Thread checkWindowsThread;
        public event EventHandler WindowListChanged;
        public event EventHandler<WindowEventArgs> WindowAdded;
        public event EventHandler<WindowEventArgs> WindowRemoved;
        public event EventHandler<WindowRectEventArgs> ActiveWindowRectChanged;
        public event EventHandler ActiveWindowChanged;
        public event EventHandler<CursorPosEventArgs> CursorPositionChanged;
        public IntPtr ActiveWindow;
        private IntPtr dockHwnd;
        public Point CursorPosition;
        public Rect ActiveWindowRect;
        private IntPtr mainWindowHandle;

        private static WindowManager manager;
        public static WindowManager Manager
        {
            // Singleton Pattern
            get { return manager ?? (manager = new WindowManager()); }
        }

        public WindowManager()
        {
            Windows = new List<Window>();
            WindowListChanged += WindowManager_WindowListChanged;
            ActiveWindowChanged += WindowManager_ActiveWindowChanged;
        }

        ~WindowManager()
        {
            Stop();
        }

        public void SetDockHwnd(IntPtr hwnd)
        {
            dockHwnd = hwnd;
        }

        public void Start(System.Windows.Window mainWindow)
        {
            Stop();
            mainWindowHandle = new WindowInteropHelper(mainWindow).Handle;
            checkWindowsThread = new Thread(() =>
            {
                while (true)
                {
                    // Check windows list
                    int windowCount = 0;
                    WindowInterop.EnumWindows((h, p) =>
                    {
                        if(isTaskBarWindow(h))
                            windowCount++;
                        return true;
                    }, 0);
                    if (windowCount != Windows.Count && WindowListChanged != null)
                        WindowListChanged(this, new EventArgs());
                    // Check active window
                    IntPtr activeWindow = WindowInterop.GetForegroundWindow();
                    if (ActiveWindow != activeWindow && ActiveWindowChanged != null && activeWindow != dockHwnd)
                        ActiveWindowChanged(activeWindow, new EventArgs());
                    // Check active window location
                    if (activeWindow != IntPtr.Zero && activeWindow != dockHwnd)
                    {
                        WindowInterop.Rect windowRect = new WindowInterop.Rect();
                        WindowInterop.GetWindowRect(activeWindow, ref windowRect);
                        if (windowRect != ActiveWindowRect && ActiveWindowRectChanged != null)
                            ActiveWindowRectChanged(this, new WindowRectEventArgs(ActiveWindowRect = windowRect));
                    }
                    // Check current cursor position
                    WindowInterop.Point cPos;
                    WindowInterop.GetCursorPos(out cPos);
                    if (CursorPosition != cPos && CursorPositionChanged != null)
                        CursorPositionChanged(this, new CursorPosEventArgs(CursorPosition = cPos));
                    Thread.Sleep(40); // ~25ips
                }
            });
            checkWindowsThread.Start();
        }

        public void Stop()
        {
            if (checkWindowsThread != null)
                checkWindowsThread.Abort();
        }

        void WindowManager_ActiveWindowChanged(object sender, EventArgs e)
        {
            if ((IntPtr)sender != mainWindowHandle)
                ActiveWindow = (IntPtr)sender;
        }

        void WindowManager_WindowListChanged(object sender, EventArgs e)
        {
            List<IntPtr> hwnds = new List<IntPtr>();
            WindowInterop.EnumWindows((h, p) =>
            {
                if(isTaskBarWindow(h))
                    hwnds.Add(h);
                return true;
            }, 0);
            List<IntPtr> chwnds = (from w in Windows select w.Hwnd).ToList();
            foreach (var h in hwnds.Except(chwnds)) // Get removed windows
            {
                Window window = new Window(h);
                Windows.Add(window);
                if (WindowAdded != null)
                    WindowAdded(this, new WindowEventArgs(window));
            }
            foreach (var h in chwnds.Except(hwnds)) // Get added windows
            {
                Window window = Windows.Find(w => w.Hwnd == h);
                if (WindowRemoved != null)
                    WindowRemoved(this, new WindowEventArgs(window));
                Windows.Remove(window);
            }
        }

        private bool isTaskBarWindow(IntPtr hwnd)
        {
            //long style = WindowInterop.GetWindowLongPtr(hwnd, WindowInterop.GWL_STYLE); // Get style.
            if (//HasFlag(style, TARGETWINDOW) &&
                WindowInterop.IsWindowVisible(hwnd))// &&
                //WindowInvoke.GetParent(hwnd) == IntPtr.Zero)
            {
                bool noOwner = WindowInterop.GetWindow(hwnd, WindowInterop.GW_OWNER) == IntPtr.Zero;
                long exStyle = WindowInterop.GetWindowLongPtr(hwnd, WindowInterop.GWL_EXSTYLE); // Get extended style.
                return noOwner && (exStyle & WS_EX_TOOLWINDOW) == 0;
            }
            return false;
        }

        private long getHwndStyleFlags(IntPtr hwnd)
        {
            return WindowInterop.GetWindowLongPtr(hwnd, WindowInterop.GWL_STYLE);
        }

        private bool HasFlag(long value, long flag)
        {
            return (value & flag) == flag;
        }
    }
}