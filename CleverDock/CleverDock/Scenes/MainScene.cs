using CleverDock.Graphics;
using CleverDock.Graphics.Views;
using CleverDock.Managers;
using CleverDock.Tools;
using CleverDock.Views;
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
        private FPSCounterView fpsCounter;
        private Dock dock;
        private WindowManager manager;

        public MainScene()
            : base(120)
        {
            manager = new WindowManager();
            manager.Start();
            View.Subviews.Add(fpsCounter = new FPSCounterView(new Rectangle(0, 0, 60, 20)));
            View.Subviews.Add(dock = new Dock());
        }


        protected override void Dispose(bool disposing)
        {
            manager.Stop();
            base.Dispose(disposing);
        }

        protected override void OnCreateResources()
        {
            this.redBrush = new D2D.SolidColorBrush(RenderTarget, new Color(1f, 0f, 0f));

            base.OnCreateResources();
        }

        protected override void OnFreeResources()
        {
            base.OnFreeResources();

            if (redBrush != null)
            {
                redBrush.Dispose();
                redBrush = null;
            }
        }

        protected override void OnRender()
        {
            this.RenderTarget.Clear(new Color(0, 0, 0, 0.0f));
            this.RenderTarget.DrawRectangle(View.Bounds, this.redBrush);

            base.OnRender();
        }
    }
}
