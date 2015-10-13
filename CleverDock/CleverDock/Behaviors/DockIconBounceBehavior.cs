using CleverDock.ViewModels;
using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CleverDock.Behaviors
{
    public class DockIconBounceBehavior : Behavior<FrameworkElement>
    {
        private IconViewModel icon;

        protected override void OnAttached()
        {
            base.OnAttached();
            icon = (IconViewModel)AssociatedObject.DataContext;
            icon.OnAnimateIconBounce += icon_OnAnimateIconBounce;
        }

        void icon_OnAnimateIconBounce(object sender, EventArgs e)
        {
            BounceIcon();
        }

        public void BounceIcon()
        {
            var translation = new DoubleAnimation
            {
                From = 0,
                To = -30,
                Duration = TimeSpan.FromSeconds(1.2),
                AutoReverse = true
            };
            translation.EasingFunction = new BounceEase
            {
                Bounces = 1,
                Bounciness = 1,
                EasingMode = EasingMode.EaseIn
            };
            Storyboard.SetTargetName(translation, "ImageTransform");
            Storyboard.SetTargetProperty(translation, new PropertyPath(TranslateTransform.YProperty));

            var sb = new Storyboard();
            sb.Children.Add(translation);
            sb.Begin(AssociatedObject);
        }
    }
}
