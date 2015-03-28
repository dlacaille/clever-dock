using CleverDock.Managers;
using CleverDock.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CleverDock
{
    /// <summary>
    /// Interaction logic for IconDragWindow.xaml
    /// </summary>
    public partial class IconDragWindow : System.Windows.Window
    {
        public IconDragWindow(IconInfo info)
        {
            InitializeComponent();
            var bitmap = IconHelper.GetIcon(info.Path, SettingsManager.Settings.IconSize);
            SetDimensions();
        }

        public void SetDimensions()
        {
            Width = Height = IconImage.Width = IconImage.Height = SettingsManager.Settings.IconSize;
        }
    }
}
