using System;
using System.Text;
using System.Threading;
using CleverDock.Interop;
using CleverDock.Managers;
using System.Windows;

namespace CleverDock.Interop
{
    public class Win32Window
    {
        public Win32Window(IntPtr _hwnd)
        {
            Hwnd = _hwnd;
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
        }

        public void Restore()
        {
            WindowInterop.SetForegroundWindow(Hwnd);
            if (IsMinimized)
                WindowInterop.ShowWindow(Hwnd, WindowInterop.ShowStyle.Restore);
        }

        public void Close()
        {
            WindowInterop.SendMessage(Hwnd, WindowInterop.WM_CLOSE, 0, 0);
        }

        public IntPtr Hwnd { get; set; }

        public string FileName 
        {
            get
            {
                return ProcessManager.GetExecutablePath(ProcessId);
            }
        }

        public int ProcessId
        {
            get
            {
                int procId;
                WindowInterop.GetWindowThreadProcessId(Hwnd, out procId);
                return procId;
            }
        }

        public bool IsActive
        {
            get
            {
                return Hwnd == WindowManager.Manager.ActiveWindow;
            }
        }

        public bool IsChild
        {
            get
            {
                return ParentHwnd != IntPtr.Zero && OwnerHwnd != IntPtr.Zero;
            }
        }

        public IntPtr ParentHwnd
        {
            get
            {
                return WindowInterop.GetParent(Hwnd);
            }
        }

        public IntPtr OwnerHwnd
        {
            get
            {
                return WindowInterop.GetWindow(Hwnd, WindowInterop.GW_OWNER);
            }
        }

        public bool IsMinimized
        {
            get
            {
                return WindowInterop.IsIconic(Hwnd);
            }
        }

        public string Title
        {
            get
            {
                StringBuilder builder = new StringBuilder(200);
                WindowInterop.GetWindowText(Hwnd, builder, builder.Capacity);
                return builder.ToString();
            }
        }

        public string ClassName
        {
            get
            {
                StringBuilder builder = new StringBuilder(200);
                WindowInterop.GetClassName(Hwnd, builder, builder.Capacity);
                return builder.ToString();
            }
        }
    }
}
