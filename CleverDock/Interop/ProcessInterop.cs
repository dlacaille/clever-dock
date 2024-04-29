using System.Runtime.InteropServices;
using System.Text;

namespace CleverDock.Interop;

internal class ProcessInterop
{
    /// <summary>
    ///     Required to retrieve certain information about a process
    /// </summary>
    public const int PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

    /// <summary>
    ///     Retrieves the full name of the executable image for the specified process.
    /// </summary>
    /// <param name="hprocess">
    ///     A handle to the process. This handle must be created with the PROCESS_QUERY_INFORMATION or
    ///     PROCESS_QUERY_LIMITED_INFORMATION access right. For more information, see Process Security and Access Rights.
    /// </param>
    /// <param name="dwFlags">
    ///     0: The name should use the Win32 path format. 1: The name should use the native system path
    ///     format.
    /// </param>
    /// <param name="lpExeName">The path to the executable image. If the function succeeds, this string is null-terminated.</param>
    /// <param name="size">
    ///     On input, specifies the size of the lpExeName buffer, in characters. On success, receives the number
    ///     of characters written to the buffer, not including the null-terminating character.
    /// </param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
    [DllImport("kernel32.dll")]
    public static extern bool QueryFullProcessImageName(IntPtr hprocess, int dwFlags,
        StringBuilder lpExeName, out int size);

    /// <summary>
    ///     Opens an existing local process object.
    /// </summary>
    /// <param name="dwDesiredAccess">
    ///     The access to the process object. This access right is checked against the security
    ///     descriptor for the process. This parameter can be one or more of the process access rights.
    /// </param>
    /// <param name="bInheritHandle">
    ///     If this value is TRUE, processes created by this process will inherit the handle.
    ///     Otherwise, the processes do not inherit this handle.
    /// </param>
    /// <param name="dwProcessId">The identifier of the local process to be opened.</param>
    /// <returns>If the function succeeds, the return value is an open handle to the specified process.</returns>
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    /// <summary>
    ///     Closes an open object handle.
    /// </summary>
    /// <param name="hHandle">A valid handle to an open object.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hHandle);

    /// <summary>
    ///     Retrieves a module handle for the specified module. The module must have been loaded by the calling process.
    /// </summary>
    /// <param name="lpModuleName">
    ///     The name of the loaded module (either a .dll or .exe file). If the file name extension is
    ///     omitted, the default library extension .dll is appended. The file name string can include a trailing point
    ///     character (.) to indicate that the module name has no extension. The string does not have to specify a path. When
    ///     specifying a path, be sure to use backslashes (\), not forward slashes (/). The name is compared (case
    ///     independently) to the names of modules currently mapped into the address space of the calling process.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is a handle to the specified module. If the function fails, the
    ///     return value is NULL.To get extended error information, call GetLastError.
    /// </returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);
}