using CleverDock.Handlers;
using CleverDock.Interop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using WI = CleverDock.Interop.WindowInterop;

namespace CleverDock.Managers
{
    public class MouseManager : IDisposable
    {
        private WI.HookProc HookCallback { get; set; }
        private IntPtr HookHandle { get; set; }

        private static MouseManager manager;
        public static MouseManager Manager
        {
            // Singleton Pattern
            get { return manager ?? (manager = new MouseManager()); }
        }

        public event EventHandler<MouseMoveEventArgs> MouseMoved;
        public event EventHandler<MouseButtonEventArgs> MouseButtonUp;
        public event EventHandler<MouseButtonEventArgs> MouseButtonDown;

        public MouseManager()
        {
            using (var process = Process.GetCurrentProcess())
            using (var module = process.MainModule)
            {
                HookCallback = HookProc;
                HookHandle = WI.SetWindowsHookEx(WI.HookType.WH_MOUSE_LL, HookCallback, ProcessInterop.GetModuleHandle(module.ModuleName), 0);
            }
        }

        public IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                return WI.CallNextHookEx(HookHandle, code, wParam, lParam);
            var wm = (WindowMessage)wParam;
            var info = Marshal.PtrToStructure<WI.MSLLHOOKSTRUCT>(lParam);
            switch (wm)
            {
                case WindowMessage.MOUSEMOVE:
                    if (MouseMoved != null)
                        MouseMoved(this, new MouseMoveEventArgs(info.pt));
                    break;
                case WindowMessage.LBUTTONUP:
                    if (MouseButtonUp != null)
                        MouseButtonUp(this, new MouseButtonEventArgs(MouseButton.Left, info.pt));
                    break;
                case WindowMessage.LBUTTONDOWN:
                    if (MouseButtonDown != null)
                        MouseButtonDown(this, new MouseButtonEventArgs(MouseButton.Left, info.pt));
                    break;
                case WindowMessage.RBUTTONUP:
                    if (MouseButtonUp != null)
                        MouseButtonUp(this, new MouseButtonEventArgs(MouseButton.Right, info.pt));
                    break;
                case WindowMessage.RBUTTONDOWN:
                    if (MouseButtonDown != null)
                        MouseButtonDown(this, new MouseButtonEventArgs(MouseButton.Right, info.pt));
                    break;
            }
            return WI.CallNextHookEx(HookHandle, code, wParam, lParam);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;
            WI.UnhookWindowsHookEx(HookHandle);
            disposedValue = true;
        }

        ~MouseManager()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
