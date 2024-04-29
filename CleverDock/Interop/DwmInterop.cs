using System.Runtime.InteropServices;

namespace CleverDock.Interop;

internal class DwmInterop
{
    public const int DWM_TNP_RECTDESTINATION = 0x00000001;
    public const int DWM_TNP_RECTSOURCE = 0x00000002;
    public const int DWM_TNP_OPACITY = 0x00000004;
    public const int DWM_TNP_VISIBLE = 0x00000008;
    public const int DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010;

    /// <summary>
    ///     Creates a Desktop Window Manager (DWM) thumbnail relationship between the destination and source windows.
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="src"></param>
    /// <param name="thumb"></param>
    /// <returns></returns>
    [DllImport("dwmapi.dll", SetLastError = true)]
    public static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

    /// <summary>
    ///     Updates the properties for a Desktop Window Manager (DWM) thumbnail.
    /// </summary>
    /// <param name="hThumbnail"></param>
    /// <param name="props"></param>
    /// <returns></returns>
    [DllImport("dwmapi.dll", PreserveSig = true)]
    public static extern int DwmUpdateThumbnailProperties(IntPtr hThumbnail, ref DWM_THUMBNAIL_PROPERTIES props);

    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_THUMBNAIL_PROPERTIES
    {
        public long dwFlags;
        public RECT rcDestination;
        public RECT rcSource;
        public byte opacity;
        public bool fVisible;
        public bool fSourceClientAreaOnly;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public long left, top, right, bottom;
    }
}