using Direct2D.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D2D = Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using DWrite = Microsoft.WindowsAPICodePack.DirectX.DirectWrite;

namespace CleverDock.Scenes
{
    class MainScene : Direct2D.AnimatedScene
    {
        private D2D.SolidColorBrush redBrush;
        private double widthRatio;
        private FPSCounterView fpsCounter;

        public MainScene()
            : base(120)
        {
            fpsCounter = new FPSCounterView(new D2D.RectF(0, 0, 60, 20));
            View.Subviews.Add(fpsCounter);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (fpsCounter != null)
            {
                fpsCounter.Dispose();
                fpsCounter = null;
            }
        }

        protected override void OnCreateResources()
        {
            this.redBrush = this.RenderTarget.CreateSolidColorBrush(new D2D.ColorF(1, 0, 0));

            base.OnCreateResources(); // Call this last to start the animation
        }

        protected override void OnFreeResources()
        {
            base.OnFreeResources(); // Call this first to stop the animation

            if (redBrush != null)
            {
                redBrush.Dispose();
                redBrush = null;
            }
        }

        protected override void OnRender()
        {

            // This is what we're going to draw. We'll animate the width of the
            // elipse over a span of five seconds (ElapsedTime / 5).
            this.widthRatio += this.ElapsedTime / 5;
            if (this.widthRatio > 1) // Reset
                this.widthRatio = 0;

            var size = this.RenderTarget.Size;
            float width = (float)((size.Width / 3.0) * this.widthRatio);
            var ellipse = new D2D.Ellipse(new D2D.Point2F(size.Width / 2.0f, size.Height / 2.0f), width, size.Height / 3.0f);

            // This draws the ellipse in red on a semi-transparent blue background
            this.RenderTarget.Clear(new D2D.ColorF(0, 0, 0, 0.0f));
            this.RenderTarget.FillEllipse(ellipse, this.redBrush);
        }
    }
}
