using System.Windows;
using System.Windows.Media;

namespace CleverDock.Tools;

public class FrameworkHelper
{
    public static T GetParent<T>(Visual v)
        where T : Visual
    {
        while (v != null)
        {
            v = VisualTreeHelper.GetParent(v) as Visual;
            if (v is T)
                break;
        }

        return v as T;
    }

    public static T GetVisualChild<T>(DependencyObject parent)
        where T : Visual
    {
        var child = default(T);
        var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < numVisuals; i++)
        {
            var v = (Visual)VisualTreeHelper.GetChild(parent, i);
            child = v as T;
            if (child == null)
                child = GetVisualChild<T>(v);
            if (child != null)
                break;
        }

        return child;
    }
}