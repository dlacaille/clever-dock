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

        protected override void LoadContent()
        {
            ToDispose(backgroundBrush = new SolidColorBrush(RenderTarget, new Color4(0.15f, 0.17f, 0.18f, 0.92f)));
            ToDispose(borderBrush = new SolidColorBrush(RenderTarget, new Color4(0f, 0f, 0f, 0.3f)));

            // Start animation.
            base.LoadContent();
        }

        public override void Draw()
        {
            base.Draw();

            var rect = new RoundedRectangle();
            rect.Rect = Frame;
            rect.RadiusX = 5;
            rect.RadiusY = 5;
            RenderTarget.DrawRoundedRectangle(rect, borderBrush, 1.0f);
            RenderTarget.FillRoundedRectangle(rect, backgroundBrush);
        }
    }
}
