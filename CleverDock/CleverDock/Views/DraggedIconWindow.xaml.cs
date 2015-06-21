using CleverDock.Interop;
using CleverDock.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CleverDock
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DraggedIconWindow : Window
    {
        public DraggedIconWindow()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            Loaded += DraggedIconWindow_Loaded;
            WindowManager.Manager.ActiveWindowChanged += Manager_ActiveWindowChanged;
        }

        void DraggedIconWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetTopmost();
        }

        void Manager_ActiveWindowChanged(object sender, EventArgs e)
        {
            SetTopmost();
        }

        public void SetTopmost()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                WindowInterop.SetWindowPos(hwnd, WindowInterop.HWND_TOPMOST, 0, 0, 0, 0, WindowInterop.SWP_NOMOVE | WindowInterop.SWP_NOSIZE);
            });
        }
    }
}
