using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleverDock.Graphics.Tools;
using SharpDX;
using D2D = SharpDX.Direct2D1;
using DXGI = SharpDX.DXGI;
using WIC = SharpDX.WIC;

namespace CleverDock.Graphics.Views
{
    public class ImageView : View
    {
        private WIC.BitmapSource bitmapSource { get; set; }
        private D2D.Bitmap bitmap { get; set; }
        private float opacity { get; set; }

        public ImageView(RectangleF bounds, WIC.BitmapSource bitmapSource)
            : base(bounds)
        {
            this.bitmapSource = bitmapSource;
            this.opacity = 1;
        }

        protected override void OnCreateResources()
        {
            var pf = new D2D.PixelFormat(DXGI.Format.B8G8R8A8_UNorm, D2D.AlphaMode.Premultiplied);
            var properties = new D2D.BitmapProperties(pf, 96.0f, 96.0f);
            bitmap = D2D.Bitmap.FromWicBitmap(RenderTarget, bitmapSource, properties);
            base.OnCreateResources();
        }

        protected override void OnFreeResources()
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }

            base.OnFreeResources();
        }

        protected override void OnRender()
        {
            base.OnRender();

            RenderTarget.DrawBitmap(bitmap, Frame, opacity, D2D.BitmapInterpolationMode.Linear); 
        }
    }
}
