using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CleverDock.Tools
{
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
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                    child = GetVisualChild<T>(v);
                if (child != null)
                    break;
            }
            return child;
        }
    }
}
