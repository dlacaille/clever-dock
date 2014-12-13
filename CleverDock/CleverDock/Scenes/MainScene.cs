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
        private D2D.SolidColorBrush whiteBrush;
        private DWrite.TextFormat textFormat;
        private DWrite.DWriteFactory writeFactory;
        private double widthRatio;

        // These are used for tracking an accurate frames per second
        private DateTime time;
        private int frameCount;
        private int fps;

        public MainScene()
            : base(100) // Will probably only be about 67 fps due to the limitations of the timer
        {
            this.writeFactory = DWrite.DWriteFactory.CreateFactory();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.writeFactory.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnCreateResources()
        {
            // We don't need to free any resources because the base class will
            // call OnFreeResources if necessary before calling this method.
            this.redBrush = this.RenderTarget.CreateSolidColorBrush(new D2D.ColorF(1, 0, 0));
            this.whiteBrush = this.RenderTarget.CreateSolidColorBrush(new D2D.ColorF(1, 1, 1));

            this.textFormat = this.writeFactory.CreateTextFormat("Arial", 12);

            base.OnCreateResources(); // Call this last to start the animation
        }

        protected override void OnFreeResources()
        {
            base.OnFreeResources(); // Call this first to stop the animation

            if (this.redBrush != null)
            {
                this.redBrush.Dispose();
                this.redBrush = null;
            }
            if (this.whiteBrush != null)
            {
                this.whiteBrush.Dispose();
                this.whiteBrush = null;
            }
            if (this.textFormat != null)
            {
                this.textFormat.Dispose();
                this.textFormat = null;
            }
        }

        protected override void OnRender()
        {
            // Calculate our actual frame rate
            this.frameCount++;
            if (DateTime.UtcNow.Subtract(this.time).TotalSeconds >= 1)
            {
                this.fps = this.frameCount;
                this.frameCount = 0;
                this.time = DateTime.UtcNow;
            }

            // This is what we're going to draw. We'll animate the width of the
            // elipse over a span of five seconds (ElapsedTime / 5).
            this.widthRatio += this.ElapsedTime / 5;
            if (this.widthRatio > 1) // Reset
            {
                this.widthRatio = 0;
            }

            var size = this.RenderTarget.Size;
            float width = (float)((size.Width / 3.0) * this.widthRatio);
            var ellipse = new D2D.Ellipse(new D2D.Point2F(size.Width / 2.0f, size.Height / 2.0f), width, size.Height / 3.0f);

            // This draws the ellipse in red on a semi-transparent blue background
            this.RenderTarget.BeginDraw();
            this.RenderTarget.Clear(new D2D.ColorF(0, 0, 1, 0.5f));
            this.RenderTarget.FillEllipse(ellipse, this.redBrush);

            // Draw a little FPS in the top left corner
            string text = string.Format("FPS {0}", this.fps);
            this.RenderTarget.DrawText(text, this.textFormat, new D2D.RectF(10, 10, 100, 20), this.whiteBrush);

            // All done!
            this.RenderTarget.EndDraw();
        }
    }
}
