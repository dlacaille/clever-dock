using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Graphics.Animations
{
    public class Animation
    {
        /// <summary>
        /// Object containing the property to be animated.
        /// </summary>
        public object AnimatedObject { get; set; }

        /// <summary>
        /// Property to be animated.
        /// </summary>
        public PropertyInfo AnimatedProperty { get; set; }

        /// <summary>
        /// Value at the start of the animation.
        /// </summary>
        public object FromValue { get; set; }

        /// <summary>
        /// Value at the end of the animation.
        /// </summary>
        public object ToValue { get; set; }

        /// <summary>
        /// Duration of the animation in seconds.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Easing of the animation.
        /// </summary>
        public EasingMode Easing { get; set; }

        /// <summary>
        /// Is true if the animation is ongoing.
        /// </summary>
        public bool IsAnimating { get; set; }
        
        private DateTime startDate { get; set; }

        /// <summary>
        /// Event called when the animation is finished.
        /// </summary>
        public event EventHandler AnimationEnded;

        public Animation() { }

        public void Animate<TSource, TResult>(TSource obj, Expression<Func<TSource, TResult>> property, TResult toValue, double duration, EasingMode easing = EasingMode.QuadraticEaseInOut)
        {
            AnimatedObject = obj;
            var member = property.Body as MemberExpression;
            AnimatedProperty = member.Member as PropertyInfo;
            ToValue = toValue;
            Duration = duration;
            Easing = easing;
            Start();
        }

        private void Interpolate(double x)
        {
            x = ApplyEasing(x, Easing);
            if (AnimatedProperty.PropertyType == typeof(double))
            {
                var from = (double)FromValue;
                var to = (double)ToValue;
                AnimatedProperty.SetValue(AnimatedObject, (to - from) * x + from);
                return;
            }
            if (AnimatedProperty.PropertyType == typeof(float))
            {
                var from = (float)FromValue;
                var to = (float)ToValue;
                AnimatedProperty.SetValue(AnimatedObject, (to - from) * (float)x + from);
                return;
            }
            if (AnimatedProperty.PropertyType == typeof(RectangleF))
            {
                var from = (RectangleF)FromValue;
                var to = (RectangleF)ToValue;
                float newX = (to.X - from.X) * (float)x + from.X;
                float newY = (to.Y - from.Y) * (float)x + from.Y;
                float newWidth = (to.Width - from.Width) * (float)x + from.Width;
                float newHeight = (to.Height - from.Height) * (float)x + from.Height;
                AnimatedProperty.SetValue(AnimatedObject, new RectangleF(newX, newY, newWidth, newHeight));
                return;
            }
            throw new NotImplementedException(string.Format("Animations on {0} are not supported.", AnimatedProperty.PropertyType.ToString()));
        }

        public double ApplyEasing(double x, EasingMode easing)
        {
            switch (easing)
            {
                case EasingMode.QuadraticEaseIn:
                    return EasingFunctions.QuadraticEaseIn(x);
                case EasingMode.QuadraticEaseOut:
                    return EasingFunctions.QuadraticEaseOut(x);
                case EasingMode.QuadraticEaseInOut:
                    return EasingFunctions.QuadraticEaseInOut(x);
                case EasingMode.CubicEaseIn:
                    return EasingFunctions.CubicEaseIn(x);
                case EasingMode.CubicEaseOut:
                    return EasingFunctions.CubicEaseOut(x);
                case EasingMode.CubicEaseInOut:
                    return EasingFunctions.CubicEaseInOut(x);
                case EasingMode.QuarticEaseIn:
                    return EasingFunctions.QuarticEaseIn(x);
                case EasingMode.QuarticEaseOut:
                    return EasingFunctions.QuarticEaseOut(x);
                case EasingMode.QuarticEaseInOut:
                    return EasingFunctions.QuarticEaseInOut(x);
                case EasingMode.QuinticEaseIn:
                    return EasingFunctions.QuinticEaseIn(x);
                case EasingMode.QuinticEaseOut:
                    return EasingFunctions.QuinticEaseOut(x);
                case EasingMode.QuinticEaseInOut:
                    return EasingFunctions.QuinticEaseInOut(x);
                case EasingMode.SineEaseIn:
                    return EasingFunctions.SineEaseIn(x);
                case EasingMode.SineEaseOut:
                    return EasingFunctions.SineEaseOut(x);
                case EasingMode.SineEaseInOut:
                    return EasingFunctions.SineEaseInOut(x);
                case EasingMode.CircularEaseIn:
                    return EasingFunctions.CircularEaseIn(x);
                case EasingMode.CircularEaseOut:
                    return EasingFunctions.CircularEaseOut(x);
                case EasingMode.CircularEaseInOut:
                    return EasingFunctions.CircularEaseInOut(x);
                case EasingMode.ExponentialEaseIn:
                    return EasingFunctions.ExponentialEaseIn(x);
                case EasingMode.ExponentialEaseOut:
                    return EasingFunctions.ExponentialEaseOut(x);
                case EasingMode.ExponentialEaseInOut:
                    return EasingFunctions.ExponentialEaseInOut(x);
                case EasingMode.ElasticEaseIn:
                    return EasingFunctions.ElasticEaseIn(x);
                case EasingMode.ElasticEaseOut:
                    return EasingFunctions.ElasticEaseOut(x);
                case EasingMode.ElasticEaseInOut:
                    return EasingFunctions.ElasticEaseInOut(x);
                case EasingMode.BackEaseIn:
                    return EasingFunctions.BackEaseIn(x);
                case EasingMode.BackEaseOut:
                    return EasingFunctions.BackEaseOut(x);
                case EasingMode.BackEaseInOut:
                    return EasingFunctions.BackEaseInOut(x);
                case EasingMode.BounceEaseIn:
                    return EasingFunctions.BounceEaseIn(x);
                case EasingMode.BounceEaseOut:
                    return EasingFunctions.BounceEaseOut(x);
                case EasingMode.BounceEaseInOut:
                    return EasingFunctions.BounceEaseInOut(x);
                default:
                    return x;
            }
        }

        public void Start()
        {
            if (IsAnimating)
                return;
            IsAnimating = true;
            startDate = DateTime.Now;
            FromValue = AnimatedProperty.GetValue(AnimatedObject);
        }

        public void Stop()
        {
            if (!IsAnimating)
                return;
            IsAnimating = false;
            if (AnimationEnded != null)
                AnimationEnded(this, new EventArgs());
        }

        public void Tick()
        {
            if (!IsAnimating)
                return;
            double x = (DateTime.Now - startDate).Ticks / (double)TimeSpan.TicksPerSecond / Duration;
            x = Math.Min(Math.Max(x, 0), 1);
            Interpolate(x);
            if (x == 1)
                Stop();
        }
    }
}
