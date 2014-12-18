using CleverDock.Graphics;
using CleverDock.Graphics.Tools;
using CleverDock.Graphics.Views;
using CleverDock.Managers;
using CleverDock.Tools;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D2D = SharpDX.Direct2D1;
using DWrite = SharpDX.DirectWrite;
using WIC = SharpDX.WIC;

namespace CleverDock.Scenes
{
    class MainScene : AnimatedScene
    {
        private D2D.SolidColorBrush redBrush;
        private double widthRatio;
        private FPSCounterView fpsCounter;
        private ImageView imgView;
        private WindowManager manager;

        public MainScene()
            : base(120)
        {
            manager = new WindowManager();
            manager.Start();

            fpsCounter = new FPSCounterView(new Rectangle(0, 0, 60, 20));
            View.Subviews.Add(fpsCounter);
            imgView = new ImageView(new RectangleF(100, 100, 48, 48), BitmapHelper.FromFile("chrome.png"));
            View.Subviews.Add(imgView);
            RectangleF newBounds = imgView.Bounds;
            newBounds.Offset(100, 0);
            Animate(imgView, v => v.Bounds, newBounds, 3);
        }


        protected override void Dispose(bool disposing)
        {
            manager.Stop();
            base.Dispose(disposing);
        }

        protected override void OnCreateResources()
        {
            this.redBrush = new D2D.SolidColorBrush(RenderTarget, new Color(1f, 0f, 0f));

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
            var size = this.RenderTarget.Size;
            float width = (float)((size.Width / 3.0) * this.widthRatio);
            var ellipse = new D2D.Ellipse(new Vector2(size.Width / 2.0f, size.Height / 2.0f), width, size.Height / 3.0f);

            // This draws the ellipse in red on a semi-transparent blue background
            this.RenderTarget.Clear(new Color(0, 0, 0, 0.0f));
            this.RenderTarget.FillEllipse(ellipse, this.redBrush);
        }
    }
}
