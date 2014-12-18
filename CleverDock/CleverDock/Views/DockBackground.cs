using CleverDock.Graphics;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Views
{
    public class DockBackground : View
    {
        private SolidColorBrush backgroundBrush { get; set; }

        public override RectangleF Frame
        {
            get
            {
                if (Superview == null)
                    return new RectangleF();
                return Superview.Frame;
            }
        }

        public DockBackground()
            : base(new RectangleF())
        {
        }

        protected override void OnCreateResources()
        {
            backgroundBrush = new SolidColorBrush(RenderTarget, new Color4(0, 1, 0, 1));

            // Start animation.
            base.OnCreateResources();
        }

        protected override void OnFreeResources()
        {
            // Stop animation.
            base.OnFreeResources();

            if (backgroundBrush != null)
            {
                backgroundBrush.Dispose();
                backgroundBrush = null;
            }
        }

        protected override void OnRender()
        {
            base.OnRender();

            RenderTarget.FillRectangle(Frame, backgroundBrush);
        }
    }
}
