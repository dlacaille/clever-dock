using System;
using System.Text;
using System.Threading;
using CleverDock.Interop;
using CleverDock.Managers;
using System.Windows;

namespace CleverDock.Model
{
    public class Window
    {
        public IntPtr Hwnd;
        public string Title;
        public int ProcId;
        public string FileName;

        private Thread titleThread;
        public event EventHandler TitleChanged;

        public Window(IntPtr _hwnd)
        {
            Hwnd = _hwnd;
            Title = GetTitle();
            titleThread = new Thread(() =>
            {
                string title = GetTitle();
                if (Title != title)
                {
                    if (TitleChanged != null)
                        TitleChanged(this, new EventArgs());
                    Title = title;
                }
            });
            titleThread.Start();
            WindowInterop.GetWindowThreadProcessId(Hwnd, out ProcId);
            FileName = ProcessManager.GetExecutablePath(ProcId);
        }

        ~Window()
        {
            if(titleThread != null)
                titleThread.Abort();
        }

        public void Toggle()
        {
            if (IsActive())
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
            if (IsMinimized())
                WindowInterop.ShowWindow(Hwnd, WindowInterop.ShowStyle.Restore);
        }

        public void Close()
        {
            WindowInterop.SendMessage(Hwnd, WindowInterop.WM_CLOSE, 0, 0);
        }

        public bool IsActive()
        {
            return Hwnd == WindowManager.Manager.LastActive;
        }

        public bool IsChild()
        {
            return GetParentHwnd() != IntPtr.Zero && GetOwnerHwnd() != IntPtr.Zero;
        }

        public IntPtr GetParentHwnd()
        {
            return WindowInterop.GetParent(Hwnd);
        }

        public IntPtr GetOwnerHwnd()
        {
            return WindowInterop.GetWindow(Hwnd, WindowInterop.GW_OWNER);
        }

        public bool IsMinimized()
        {
            return WindowInterop.IsIconic(Hwnd);
        }

        private string GetTitle()
        {
            StringBuilder builder = new StringBuilder(200);
            WindowInterop.GetWindowText(Hwnd, builder, builder.Capacity);
            return builder.ToString();
        }
    }
}
