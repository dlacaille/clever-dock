using CleverDock.Scenes;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainScene scene = new MainScene();

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                this.Direct2DControl.Scene = this.scene;
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to create a Direct2D and/or Direct3D device.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
            }
            this.scene.IsAnimating = true;
        }
    }
}
