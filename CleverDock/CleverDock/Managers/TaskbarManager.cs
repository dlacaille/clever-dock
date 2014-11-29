using CleverDock.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Managers
{
    public static class TaskbarManager
    {
        public static void SetTaskbarVisibility(bool visible)
        {
            var taskBarHwnd = WindowInterop.FindWindow("Shell_TrayWnd", "");
            var startHwnd = WindowInterop.FindWindowEx(taskBarHwnd, IntPtr.Zero, "Button", "Start");
            if (startHwnd == IntPtr.Zero)
            {
                // Tente de trouver le bouton démarrer de Vista/Win7/Win8
                startHwnd = GetVistaStartMenuHwnd(taskBarHwnd);
            }
            var showStyle = visible ? WindowInterop.ShowStyle.Show : WindowInterop.ShowStyle.Hide;
            WindowInterop.ShowWindow(taskBarHwnd, showStyle);
            WindowInterop.ShowWindow(startHwnd, showStyle);
        }

        private static IntPtr vistaStartMenuWnd = IntPtr.Zero;
        private static IntPtr GetVistaStartMenuHwnd(IntPtr taskBarHwnd)
        {
            int procId;
            WindowInterop.GetWindowThreadProcessId(taskBarHwnd, out procId);

            Process p = Process.GetProcessById(procId);
            if (p != null)
            {
                foreach (ProcessThread t in p.Threads)
                    WindowInterop.EnumThreadWindows(t.Id, EnumThreadWindowsCallback, IntPtr.Zero);
            }
            return vistaStartMenuWnd;
        }

        private static bool EnumThreadWindowsCallback(IntPtr Hwnd, IntPtr lParam)
        {
            StringBuilder buffer = new StringBuilder(256);
            var possibleTitles = new string[] { "Start", "Démarrer" };
            if (WindowInterop.GetWindowText(Hwnd, buffer, buffer.Capacity) > 0)
            {
                Console.WriteLine(buffer);
                if (possibleTitles.Contains(buffer.ToString()))
                {
                    vistaStartMenuWnd = Hwnd;
                    return false;
                }
            }
            return true;
        }
    }
}
