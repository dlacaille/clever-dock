using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace CleverDock.Tools
{
    public class AnimationTools
    {
        public static void FadeOut(double seconds, FrameworkElement element, double target = 0, Action completed = null)
        {
            var fadeOut = new DoubleAnimation
            {
                    From = 1,
                    To = target,
                    Duration = TimeSpan.FromSeconds(seconds)
            };
            BeginAnimation(fadeOut, element, UIElement.OpacityProperty, completed);
        }

        public static void ExpandX(double seconds, int finalWidth, FrameworkElement element, Action completed = null, double delay = 0)
        {
            var collapse = new DoubleAnimation
            {
                    To = finalWidth,
                    Duration = TimeSpan.FromSeconds(seconds),
                    BeginTime = TimeSpan.FromSeconds(delay),
                    EasingFunction = new QuadraticEase()
            };
            BeginAnimation(collapse, element, FrameworkElement.WidthProperty, completed);
        }

        public static void CollapseX(double seconds, FrameworkElement element, Action completed = null)
        {
            var collapse = new DoubleAnimation
            {
                    To = 0,
                    Duration = TimeSpan.FromSeconds(seconds),
                    EasingFunction = new QuadraticEase()
            };
            BeginAnimation(collapse, element, FrameworkElement.WidthProperty, completed);
        }

        public static void TranslateY(double seconds, double to, DependencyProperty property, FrameworkElement element, Action completed = null)
        {
            var collapse = new DoubleAnimation
            {
                To = to,
                Duration = TimeSpan.FromSeconds(seconds),
                EasingFunction = new QuadraticEase()
            };
            BeginAnimation(collapse, element, property, completed);
        }

        public static void BeginAnimation(Timeline animation, FrameworkElement element, DependencyProperty property,
                                          Action completed = null)
        {
            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, new PropertyPath(property));
            var sb = new Storyboard();
            sb.Children.Add(animation);
            if (completed != null)
                sb.Completed += (s, e) => {
                    completed.Invoke();
                };
            sb.Begin(element);
        }
    }
}