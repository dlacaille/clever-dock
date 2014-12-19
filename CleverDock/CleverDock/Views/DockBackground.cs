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
        private SolidColorBrush borderBrush { get; set; }

        public override RectangleF Frame
        {
            get
            {
                if (Superview == null)
                    return new RectangleF();
                var frame = Superview.Frame;
                frame.Inflate(10, 8);
                return frame;
            }
        }

        protected override void OnCreateResources()
        {
            backgroundBrush = new SolidColorBrush(RenderTarget, new Color4(0.10f, 0.12f, 0.13f, 0.92f));
            borderBrush = new SolidColorBrush(RenderTarget, new Color4(0f, 0f, 0f, 0.3f));

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
            if (borderBrush != null)
            {
                borderBrush.Dispose();
                borderBrush = null;
            }
        }

        protected override void OnRender()
        {
            base.OnRender();

            var rect = new RoundedRectangle();
            rect.Rect = Frame;
            rect.RadiusX = 5;
            rect.RadiusY = 5;
            RenderTarget.DrawRoundedRectangle(rect, borderBrush, 1.0f);
            RenderTarget.FillRoundedRectangle(rect, backgroundBrush);
        }
    }
}
