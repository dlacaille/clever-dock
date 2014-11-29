using System;
using System.Text;
using CleverDock.Interop;

namespace CleverDock.Managers
{
    class ProcessManager
    {
        // Vista +
        public static string GetExecutablePath(int dwProcessId)
        {
            StringBuilder buffer = new StringBuilder(1024);
            IntPtr hprocess = ProcessInterop.OpenProcess(ProcessInterop.PROCESS_QUERY_LIMITED_INFORMATION, false, dwProcessId);
            if (hprocess == IntPtr.Zero)
                return string.Empty;
            try
            {
                int size = buffer.Capacity;
                if (ProcessInterop.QueryFullProcessImageName(hprocess, 0, buffer, out size))
                    return buffer.ToString();
            }
            finally
            { 
                ProcessInterop.CloseHandle(hprocess);
            }
            return string.Empty;
        }
    }
}
