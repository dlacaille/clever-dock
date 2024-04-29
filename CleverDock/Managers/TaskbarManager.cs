﻿using System.Diagnostics;
using System.Text;
using CleverDock.Interop;

namespace CleverDock.Managers;

public static class TaskbarManager
{
    private static IntPtr vistaStartMenuWnd = IntPtr.Zero;

    public static void SetTaskbarVisibility(bool visible)
    {
        var taskBarHwnd = WindowInterop.FindWindow("Shell_TrayWnd", "");
        var startHwnd = WindowInterop.FindWindowEx(taskBarHwnd, IntPtr.Zero, "Button", "Start");
        if (startHwnd == IntPtr.Zero)
            startHwnd = GetVistaStartMenuHwnd(taskBarHwnd);
        var showStyle = visible ? WindowInterop.ShowStyle.Show : WindowInterop.ShowStyle.Hide;
        WindowInterop.ShowWindow(taskBarHwnd, showStyle);
        WindowInterop.ShowWindow(startHwnd, showStyle);
    }

    private static IntPtr GetVistaStartMenuHwnd(IntPtr taskBarHwnd)
    {
        int procId;
        WindowInterop.GetWindowThreadProcessId(taskBarHwnd, out procId);

        var p = Process.GetProcessById(procId);
        if (p != null)
            foreach (ProcessThread t in p.Threads)
                WindowInterop.EnumThreadWindows(t.Id, EnumThreadWindowsCallback, IntPtr.Zero);
        return vistaStartMenuWnd;
    }

    private static bool EnumThreadWindowsCallback(IntPtr Hwnd, IntPtr lParam)
    {
        var buffer = new StringBuilder(256);
        var possibleTitles = new[] { "Start", "Démarrer" };
        if (WindowInterop.GetWindowText(Hwnd, buffer, buffer.Capacity) > 0)
            if (possibleTitles.Contains(buffer.ToString()))
            {
                vistaStartMenuWnd = Hwnd;
                return false;
            }

        return true;
    }
}