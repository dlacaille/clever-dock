using System.Windows;
using System.Windows.Media.Animation;

namespace CleverDock
{
    /// <summary>
    ///   Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Change Timeline frame rate to 60fps.
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 60 });
        }
    }
}