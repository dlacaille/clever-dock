using CleverDock.Graphics.Animations;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CleverDock.Graphics.Scenes
{
    /// <summary>
    /// Allows drawing of a Scene at a specified Frame Per Second.
    /// </summary>
    public abstract class AnimatedScene : Scene
    {
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;
        private List<Animation> animations = new List<Animation>();
        private Task renderLoopTask;
        private double msPerFrame;

        /// <summary>
        /// Initializes a new instance of the AnimatedScene class.
        /// </summary>
        /// <param name="desiredFps">
        /// The desired number of frames to render per second.
        /// </param>
        /// <remarks>
        /// The desiredFps parameter must be greater than zero. Setting the value to
        /// <see cref="Int32.MaxValue">int.MaxValue</see> will cause the control to
        /// update as often as possible.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// desiredFps is less than or equal to zero.
        /// </exception>
        protected AnimatedScene(int desiredFps)
        {
            msPerFrame = 1000.0 / desiredFps;
        }

        protected void RenderLoop()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var start = DateTime.Now;
                // Update animations.
                for (int i = 0; i < animations.Count; i++)
                    if (animations[i].IsAnimating)
                        animations[i].Tick();
                this.Render(); // Force an update.
                double elapsed = (DateTime.Now - start).TotalMilliseconds;
                Thread.Sleep((int)Math.Max(msPerFrame - elapsed, 0));
            }
        }

        protected void StartAnimation()
        {
            Console.WriteLine("Starting animation.");
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            renderLoopTask = Task.Factory.StartNew(RenderLoop, cancellationToken);
        }

        protected void StopAnimation()
        {
            if (renderLoopTask == null)
                return;
            Console.WriteLine("Stopping animation.");
            cancellationTokenSource.Cancel();
            renderLoopTask.Wait();
            renderLoopTask = null;
        }

        /// <summary>
        /// Creates device dependent resources and resumes the animation.
        /// </summary>
        protected override void OnCreateResources()
        {
            base.OnCreateResources();
            StartAnimation();
        }

        /// <summary>
        /// Releases device dependent resources and pauses the animation.
        /// </summary>
        protected override void OnFreeResources()
        {
            base.OnFreeResources();
            StopAnimation();
        }
        
        public void Animate<TSource, TResult>(TSource obj, Expression<Func<TSource, TResult>> property, TResult toValue, double duration, EasingMode easing = EasingMode.QuadraticEaseInOut, EventHandler animationEnded = null)
        {
            var animation = new Animation();
            animation.Animate(obj, property, toValue, duration, easing);
            if (animationEnded != null)
                animation.AnimationEnded += animationEnded;
            animation.AnimationEnded += (s, e) =>
            {
                animations.Remove(s as Animation);
                /*if (animations.Count == 0)
                    IsAnimating = false;*/
            };
            animations.Add(animation);
            //IsAnimating = true;
        }
    }
}
