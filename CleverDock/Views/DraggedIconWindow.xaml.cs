using System.Windows;
using System.Windows.Interop;
using CleverDock.Interop;
using CleverDock.Managers;

namespace CleverDock;

/// <summary>
///     Interaction logic for Window1.xaml
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

    private void DraggedIconWindow_Loaded(object sender, RoutedEventArgs e)
    {
        SetTopmost();
    }

    private void Manager_ActiveWindowChanged(object sender, EventArgs e)
    {
        SetTopmost();
    }

    public void SetTopmost()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowInterop.SetWindowPos(hwnd, WindowInterop.HWND_TOPMOST, 0, 0, 0, 0,
                WindowInterop.SWP_NOMOVE | WindowInterop.SWP_NOSIZE);
        });
    }
}