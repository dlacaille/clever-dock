using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using CleverDock.Handlers;
using CleverDock.Interop;
using CleverDock.Patterns;
using CleverDock.Tools;
using CleverDock.ViewModels;

namespace CleverDock.Managers;

public class WindowManager
{
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

    private static WindowManager manager;
    public IntPtr ActiveWindow;
    public Rect ActiveWindowRect;

    private Thread checkWindowsThread;
    private Thread checkWorkingAreaThread;
    private IntPtr dockHwnd;

    public WindowManager()
    {
        Windows = new List<Win32Window>();
        WindowListChanged += WindowManager_WindowListChanged;
        ActiveWindowChanged += WindowManager_ActiveWindowChanged;
        WindowAdded += WindowManager_WindowAdded;
        WindowRemoved += WindowManager_WindowRemoved;
        Application.Current.Exit += Current_Exit;
    }

    public List<Win32Window> Windows { get; set; }

    public static WindowManager Manager =>
        // Singleton Pattern
        manager ?? (manager = new WindowManager());

    public event EventHandler WorkingAreaChanged;
    public event EventHandler WindowListChanged;
    public event EventHandler<WindowEventArgs> WindowAdded;
    public event EventHandler<WindowEventArgs> WindowRemoved;
    public event EventHandler<WindowRectEventArgs> ActiveWindowRectChanged;
    public event EventHandler ActiveWindowChanged;

    private void Current_Exit(object sender, ExitEventArgs e)
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
            var rect = WorkAreaManager.GetWorkingArea();
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
                var windowCount = 0;
                WindowInterop.EnumWindows((h, p) =>
                {
                    if (isTaskBarWindow(h))
                        windowCount++;
                    return true;
                }, 0);
                if (windowCount != Windows.Count && WindowListChanged != null)
                    WindowListChanged(this, new EventArgs());
                // Check active window
                var activeHwnd = WindowInterop.GetForegroundWindow();
                var activeWindow = new Win32Window(activeHwnd);
                var isDock = activeWindow.FileName == Process.GetCurrentProcess().MainModule.FileName;
                if (ActiveWindow != activeHwnd && ActiveWindowChanged != null && !isDock)
                    ActiveWindowChanged(activeHwnd, new EventArgs());
                // Check active window location
                if (activeHwnd != IntPtr.Zero && !isDock)
                {
                    var windowRect = new WindowInterop.Rect();
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
            checkWorkingAreaThread.Interrupt();
        if (checkWindowsThread != null)
            checkWindowsThread.Interrupt();
    }

    private void WindowManager_ActiveWindowChanged(object sender, EventArgs e)
    {
        ActiveWindow = (IntPtr)sender;
    }

    private void WindowManager_WindowListChanged(object sender, EventArgs e)
    {
        var hwnds = new List<IntPtr>();
        WindowInterop.EnumWindows((h, p) =>
        {
            if (isTaskBarWindow(h))
                hwnds.Add(h);
            return true;
        }, 0);
        var chwnds = (from w in Windows select w.Hwnd).ToList();
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

    private void WindowManager_WindowRemoved(object sender, WindowEventArgs e)
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

    private void WindowManager_WindowAdded(object sender, WindowEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
        {
            var foundItems = VMLocator.Main.Icons.Where(i => PathTools.SamePath(i.Path, e.Window.FileName));
            IconViewModel windowIcon = null;
            if (foundItems.Any())
                windowIcon = foundItems.First();
            if (!foundItems.Any() || (windowIcon != null && windowIcon.Windows.Count > 0))
            {
                windowIcon = new IconViewModel
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
        if ( /*HasFlag(style, TARGETWINDOW) &&*/
            WindowInterop.IsWindowVisible(hwnd) /*&&
            WindowInterop.GetParent(hwnd) == IntPtr.Zero*/)
        {
            var window = new Win32Window(hwnd);
            var noOwner = WindowInterop.GetWindow(hwnd, WindowInterop.GW_OWNER) == IntPtr.Zero;
            var exStyle = WindowInterop.GetWindowLongPtr(hwnd, WindowInterop.GWL_EXSTYLE); // Get extended style.
            var isWin10App = window.ClassName == "ApplicationFrameWindow";
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