using CleverDock.Graphics;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Views
{
    public class DockBackground : View
    {
        public DockBackground(Scene scene)
            : base(scene)
        {

        }

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

        public override void Draw(GameTime gameTime)
        {
            var rect = new RoundedRectangle();
            rect.Rect = Frame;
            rect.RadiusX = 5;
            rect.RadiusY = 5;
            /*RenderTarget.DrawRoundedRectangle(rect, borderBrush, 1.0f);
            RenderTarget.FillRoundedRectangle(rect, backgroundBrush);*/
        }
    }
}
