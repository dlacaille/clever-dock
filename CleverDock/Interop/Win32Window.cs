using System.Text;
using CleverDock.Managers;

namespace CleverDock.Interop;

public class Win32Window
{
    public Win32Window(IntPtr _hwnd)
    {
        Hwnd = _hwnd;
    }

    public IntPtr Hwnd { get; set; }

    public string FileName => ProcessManager.GetExecutablePath(ProcessId);

    public int ProcessId
    {
        get
        {
            int procId;
            WindowInterop.GetWindowThreadProcessId(Hwnd, out procId);
            return procId;
        }
    }

    public bool IsActive => Hwnd == WindowManager.Manager.ActiveWindow;

    public bool IsChild => ParentHwnd != IntPtr.Zero && OwnerHwnd != IntPtr.Zero;

    public IntPtr ParentHwnd => WindowInterop.GetParent(Hwnd);

    public IntPtr OwnerHwnd => WindowInterop.GetWindow(Hwnd, WindowInterop.GW_OWNER);

    public bool IsMinimized => WindowInterop.IsIconic(Hwnd);

    public string Title
    {
        get
        {
            var builder = new StringBuilder(200);
            WindowInterop.GetWindowText(Hwnd, builder, builder.Capacity);
            return builder.ToString();
        }
    }

    public string ClassName
    {
        get
        {
            var builder = new StringBuilder(200);
            WindowInterop.GetClassName(Hwnd, builder, builder.Capacity);
            return builder.ToString();
        }
    }

    public void Toggle()
    {
        if (IsActive)
            Minimize();
        else
            Restore();
    }

    public void Minimize()
    {
        WindowInterop.ShowWindow(Hwnd, WindowInterop.ShowStyle.Minimize);
        if (IsActive)
            WindowManager.Manager.ActiveWindow = IntPtr.Zero;
    }

    public void Restore()
    {
        WindowInterop.SetForegroundWindow(Hwnd);
        if (IsMinimized)
            WindowInterop.ShowWindow(Hwnd, WindowInterop.ShowStyle.Restore);
    }

    public void Close()
    {
        WindowInterop.SendMessage(Hwnd, WindowMessage.CLOSE, 0, 0);
    }
}