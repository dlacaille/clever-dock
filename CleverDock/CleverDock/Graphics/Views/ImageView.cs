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
            var converter = new SharpDX.WIC.FormatConverter(new WIC.ImagingFactory());
            converter.Initialize(bitmapSource, WIC.PixelFormat.Format32bppPBGRA);
            bitmap = D2D.Bitmap.FromWicBitmap(RenderTarget, converter);

            // Start animation.
            base.OnCreateResources();
        }

        protected override void OnFreeResources()
        {
            base.OnFreeResources();

            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
        }

        protected override void OnRender()
        {
            base.OnRender();

            RenderTarget.DrawBitmap(bitmap, Frame, opacity, D2D.BitmapInterpolationMode.Linear); 
        }
    }
}
