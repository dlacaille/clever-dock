using CleverDock.Graphics.Animations;
using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CleverDock.Graphics
{
    /// <summary>
    /// Allows drawing of a Scene with animations.
    /// </summary>
    public abstract class AnimatedScene : Scene
    {
        private List<Animation> animations = new List<Animation>();

        protected override bool BeginDraw()
        {
            for (int i = 0; i < animations.Count; i++)
                if (animations[i].IsAnimating)
                    animations[i].Tick();
            return base.BeginDraw();
        }
        
        public void Animate<TSource, TResult>(TSource obj, Expression<Func<TSource, TResult>> property, TResult toValue, double duration, EasingMode easing = EasingMode.QuadraticEaseInOut, EventHandler animationEnded = null)
        {
            var animation = new Animation();
            animation.Animate(obj, property, toValue, duration, easing);
            if (animationEnded != null)
                animation.AnimationEnded += animationEnded;
            animation.AnimationEnded += (s, e) => animations.Remove(s as Animation);
            animations.Add(animation);
        }
    }
}
