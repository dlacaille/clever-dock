using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using D2D = SharpDX.Direct2D1;
using DXGI = SharpDX.DXGI;
using WIC = SharpDX.WIC;

namespace CleverDock.Graphics.Views
{
    public class ImageView : View
    {
        private WIC.BitmapSource bitmapSource;
        public WIC.BitmapSource BitmapSource 
        { 
            get 
            {
                return bitmapSource;
            }
            set
            {
                bitmapSource = value;
                CreateBitmapFromSource();
            } 
        }
        private D2D.Bitmap bitmap { get; set; }
        private float opacity { get; set; }

        public ImageView(RectangleF bounds, WIC.BitmapSource bitmapSource)
        {
            this.Bounds = bounds;
            this.bitmapSource = bitmapSource;
            this.opacity = 1;
        }

        protected override void OnCreateResources()
        {
            CreateBitmapFromSource();

            // Start animation.
            base.OnCreateResources();
        }

        private void CreateBitmapFromSource()
        {
            if (RenderTarget == null || bitmapSource == null)
                return;
            var converter = new SharpDX.WIC.FormatConverter(new WIC.ImagingFactory());
            converter.Initialize(bitmapSource, WIC.PixelFormat.Format32bppPBGRA);
            bitmap = D2D.Bitmap.FromWicBitmap(RenderTarget, converter);
        }

        protected override void OnFreeResources()
        {
            // Stop animation.
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
            if (bitmap == null)
                return;
            RenderTarget.DrawBitmap(bitmap, Frame, opacity, D2D.BitmapInterpolationMode.Linear); 
        }
    }
}
