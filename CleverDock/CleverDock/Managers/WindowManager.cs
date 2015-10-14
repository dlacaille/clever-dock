using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using CleverDock.Handlers;
using CleverDock.Interop;
using System.Diagnostics;
using System.Text;
using System.Windows.Threading;
using CleverDock.Patterns;
using CleverDock.Tools;
using CleverDock.ViewModels;
using System.IO;

namespace CleverDock.Managers
{
    public class WindowManager
    {
        public List<Win32Window> Windows { get; set; }

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
        private Thread checkWorkingAreaThread;
        public event EventHandler WorkingAreaChanged;
        public event EventHandler WindowListChanged;
        public event EventHandler<WindowEventArgs> WindowAdded;
        public event EventHandler<WindowEventArgs> WindowRemoved;
        public event EventHandler<WindowRectEventArgs> ActiveWindowRectChanged;
        public event EventHandler ActiveWindowChanged;
        public IntPtr ActiveWindow;
        private IntPtr dockHwnd;
        public Rect ActiveWindowRect;

        private static WindowManager manager;
        public static WindowManager Manager
        {
            // Singleton Pattern
            get { return manager ?? (manager = new WindowManager()); }
        }

        public WindowManager()
        {
            Windows = new List<Win32Window>();
            WindowListChanged += WindowManager_WindowListChanged;
            ActiveWindowChanged += WindowManager_ActiveWindowChanged;
            WindowAdded += WindowManager_WindowAdded;
            WindowRemoved += WindowManager_WindowRemoved;
            Application.Current.Exit += Current_Exit;
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            Stop();
        }

        ~WindowManager()
        {
            Stop();
        }

        public void SetDockHwnd(IntPtr hwnd)
        {
            dockHwnd = hwnd;
        }

        public void Start()
        {
            Stop();
            checkWorkingAreaThread = new Thread(() =>
            {
                SystemInterop.RECT rect = WorkAreaManager.GetWorkingArea();
                while (true)
                {
                    var newRect = WorkAreaManager.GetWorkingArea();
                    if (rect.Bottom != newRect.Bottom
                        || rect.Top != newRect.Top
                        || rect.Left != newRect.Left
                        || rect.Right != newRect.Right)
                    {
                        if (WorkingAreaChanged != null)
                            WorkingAreaChanged(this, new EventArgs());
                        rect = newRect;
                    }
                    Thread.Sleep(100); // ~10ips
                }
            });
            checkWorkingAreaThread.Start();
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
                    IntPtr activeHwnd = WindowInterop.GetForegroundWindow();
                    var activeWindow = new Win32Window(activeHwnd);
                    var isDock = activeWindow.FileName == Process.GetCurrentProcess().MainModule.FileName;
                    if (ActiveWindow != activeHwnd && ActiveWindowChanged != null && !isDock)
                        ActiveWindowChanged(activeHwnd, new EventArgs());
                    // Check active window location
                    if (activeHwnd != IntPtr.Zero && !isDock)
                    {
                        WindowInterop.Rect windowRect = new WindowInterop.Rect();
                        WindowInterop.GetWindowRect(activeHwnd, ref windowRect);
                        if (windowRect != ActiveWindowRect && ActiveWindowRectChanged != null)
                            ActiveWindowRectChanged(this, new WindowRectEventArgs(ActiveWindowRect = windowRect));
                    }
                    Thread.Sleep(50); // ~20ips
                }
            });
            checkWindowsThread.Start();
        }

        public void Stop()
        {
            if (checkWorkingAreaThread != null)
                checkWorkingAreaThread.Abort();
            if (checkWindowsThread != null)
                checkWindowsThread.Abort();
        }

        void WindowManager_ActiveWindowChanged(object sender, EventArgs e)
        {
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
                var window = new Win32Window(h);
                Windows.Add(window);
                if (WindowAdded != null)
                    WindowAdded(this, new WindowEventArgs(window));
            }
            foreach (var h in chwnds.Except(hwnds)) // Get added windows
            {
                var window = Windows.Find(w => w.Hwnd == h);
                if (WindowRemoved != null)
                    WindowRemoved(this, new WindowEventArgs(window));
                Windows.Remove(window);
            }
        }

        void WindowManager_WindowRemoved(object sender, WindowEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                var foundItem = VMLocator.Main.Icons.FirstOrDefault(i => i.Windows.Any(w => w.Hwnd == e.Window.Hwnd));
                if (foundItem != null)
                {
                    foundItem.Windows.Remove(e.Window);
                    if (!foundItem.Windows.Any() && !foundItem.Pinned)
                        VMLocator.Main.Icons.Remove(foundItem);
                }
            }));
        }

        void WindowManager_WindowAdded(object sender, WindowEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                var foundItems = VMLocator.Main.Icons.Where(i => PathTools.SamePath(i.Path, e.Window.FileName));
                IconViewModel windowIcon = null;
                if (foundItems.Any())
                    windowIcon = foundItems.First();
                if (!foundItems.Any() || (windowIcon != null && windowIcon.Windows.Count > 0))
                {
                    windowIcon = new IconViewModel()
                    {
                        Name = Path.GetFileName(e.Window.FileName),
                        Path = e.Window.FileName,
                        Pinned = false
                    };
                    VMLocator.Main.Icons.Add(windowIcon);
                }
                windowIcon.Windows.Add(e.Window);
            }));
        }

        private bool isTaskBarWindow(IntPtr hwnd)
        {
            //long style = WindowInterop.GetWindowLongPtr(hwnd, WindowInterop.GWL_STYLE); // Get style.
            if (/*HasFlag(style, TARGETWINDOW) &&*/
                WindowInterop.IsWindowVisible(hwnd) /*&&
                WindowInterop.GetParent(hwnd) == IntPtr.Zero*/)
            {
                var window = new Win32Window(hwnd);
                bool noOwner = WindowInterop.GetWindow(hwnd, WindowInterop.GW_OWNER) == IntPtr.Zero;
                long exStyle = WindowInterop.GetWindowLongPtr(hwnd, WindowInterop.GWL_EXSTYLE); // Get extended style.
                bool isWin10App = window.ClassName == "ApplicationFrameWindow";
                return noOwner && (exStyle & WS_EX_TOOLWINDOW) == 0 && !isWin10App;
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