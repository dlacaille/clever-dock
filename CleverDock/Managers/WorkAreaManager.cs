using CleverDock.Interop;

namespace CleverDock.Managers;

internal class WorkAreaManager
{
    public static void SetWorkingArea(int left, int top, int right, int bottom)
    {
        var rect = new SystemInterop.RECT { Left = left, Top = top, Right = right, Bottom = bottom };
        SystemInterop.SystemParametersInfo(SystemInterop.SPI.SPI_SETWORKAREA, 0, ref rect,
            SystemInterop.SPIF.SPIF_UPDATEINIFILE);
    }

    public static SystemInterop.RECT GetWorkingArea()
    {
        var rect = new SystemInterop.RECT();
        SystemInterop.SystemParametersInfo(SystemInterop.SPI.SPI_GETWORKAREA, 0, ref rect,
            SystemInterop.SPIF.SPIF_UPDATEINIFILE);
        return rect;
    }
}