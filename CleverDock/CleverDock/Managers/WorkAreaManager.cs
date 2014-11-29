using CleverDock.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Managers
{
    class WorkAreaManager
    {
        public static void SetWorkingArea(int left, int top, int right, int bottom)
        {
            IntPtr ptr = IntPtr.Zero;
            SystemInterop.RECT rect = new SystemInterop.RECT() { Left = left, Top = top, Right = right, Bottom = bottom };
            bool success = SystemInterop.SystemParametersInfo(SystemInterop.SPI.SPI_SETWORKAREA, 0, ref rect, SystemInterop.SPIF.SPIF_CHANGE);
            if(!success)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
