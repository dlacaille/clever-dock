using System.Windows;

namespace CleverDock.Helpers;

public static class ScreenHelper
{
    public static Size GetScreenResolution(bool transformToDevice = false)
    {
        if (transformToDevice)
        {
            if (Application.Current.MainWindow == null)
                throw new Exception("Cannot find the main window");
            var ps = PresentationSource.FromVisual(Application.Current.MainWindow);
            if (ps?.CompositionTarget == null)
                throw new Exception("Cannot find a valid presentation source");
            var m = ps.CompositionTarget.TransformToDevice;
            return new Size(
                SystemParameters.PrimaryScreenWidth * m.M22,
                SystemParameters.PrimaryScreenHeight * m.M11
            );
        }

        return new Size(
            SystemParameters.PrimaryScreenWidth,
            SystemParameters.PrimaryScreenHeight
        );
    }
}