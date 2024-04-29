using System.Text;
using CleverDock.Interop;

namespace CleverDock.Managers;

public class ProcessManager
{
    // Vista +
    public static string GetExecutablePath(int dwProcessId)
    {
        var buffer = new StringBuilder(1024);
        var hprocess = ProcessInterop.OpenProcess(ProcessInterop.PROCESS_QUERY_LIMITED_INFORMATION, false, dwProcessId);
        if (hprocess == IntPtr.Zero)
            return string.Empty;
        try
        {
            var size = buffer.Capacity;
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