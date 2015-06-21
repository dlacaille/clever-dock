using CleverDock.Helpers;
using CleverDock.Managers;
using CleverDock.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CleverDock.Behaviors
{
    public class DockAutoHideBehavior : Behavior<FrameworkElement>
    {
        private const double HotspotHeight = 10;

        private bool Hidden = false;
        private Storyboard Storyboard = null;

        protected override void OnAttached()
        {
            base.OnAttached();
            WindowManager.Manager.ActiveWindowRectChanged += Manager_ActiveWindowRectChanged;
            WindowManager.Manager.CursorPositionChanged += Manager_CursorPositionChanged;
        }

        void Manager_CursorPositionChanged(object sender, Handlers.CursorPosEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Hidden && MouseHotspot.Contains(WindowManager.Manager.CursorPosition))
                    Show();
            });
        }

        void Manager_ActiveWindowRectChanged(object sender, Handlers.WindowRectEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                HideOrShowDock();
            });
        }

        private void HideOrShowDock()
        {
            var window = DockManager.Manager.Window;
            var dockPos = AssociatedObject.PointToScreen(new Point(0, 0));
            var dockHeight = AssociatedObject.ActualHeight;
            var dockRect = new Rect(dockPos.X, window.Top + window.Height - dockHeight, AssociatedObject.ActualWidth, dockHeight);
            var intersects = WindowManager.Manager.ActiveWindowRect.IntersectsWith(dockRect);
            //Console.WriteLine("X:{0} Y:{1} W:{2} H:{3} Intersects:{4}", dockRect.Left, dockRect.Top, dockRect.Width, dockRect.Height, intersects ? "True" : "False");
            if (intersects)
                Hide();
            else
                Show();
        }

        private Rect MouseHotspot
        {
            get
            {
                var window = DockManager.Manager.Window;
                double hotspotWidth = Math.Max(window.Width, ScreenHelper.ScreenWidth / 2);
                double hotspotLeft = (ScreenHelper.ScreenWidth - hotspotWidth) / 2;
                double hotspotHeight = Hidden ? window.Height : HotspotHeight;
                return new Rect(hotspotLeft, ScreenHelper.ScreenHeight - hotspotHeight, hotspotWidth, hotspotHeight);
            }
        }

        private void AnimateY(float to, TimeSpan duration, Action completed = null)
        {
            // Stop the previous animation
            if (Storyboard != null)
                Storyboard.Stop();

            var translation = new DoubleAnimation(to, duration);
            translation.EasingFunction = new QuadraticEase
            {
                 EasingMode = EasingMode.EaseInOut
            };
            Storyboard.SetTargetName(translation, "DockTransform");
            Storyboard.SetTargetProperty(translation, new PropertyPath(TranslateTransform.YProperty));

            Storyboard = new Storyboard();
            Storyboard.Children.Add(translation);
            Storyboard.Begin(AssociatedObject);
            if (completed != null)
                Storyboard.Completed += (s, e) =>
                {
                    Storyboard = null;
                    completed.Invoke();
                };
        }

        private void Hide()
        {
            if (Hidden)
                return;
            Hidden = true;
            AnimateY(100, TimeSpan.FromSeconds(0.5));
        }

        private void Show()
        {
            if (!Hidden)
                return;
            Hidden = false;
            AnimateY(0, TimeSpan.FromSeconds(0.5));
        }

    }
}
