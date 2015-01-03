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

        private void CreateBitmapFromSource()
        {
            if (RenderTarget == null || bitmapSource == null)
                return;
            var converter = new SharpDX.WIC.FormatConverter(new WIC.ImagingFactory());
            converter.Initialize(bitmapSource, WIC.PixelFormat.Format32bppPBGRA);
            bitmap = D2D.Bitmap.FromWicBitmap(RenderTarget, converter);
        }

        protected override void LoadContent()
        {
            CreateBitmapFromSource();

            // Start animation.
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            // Stop animation.
            base.UnloadContent();

            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (bitmap == null)
                return;
            RenderTarget.DrawBitmap(bitmap, Frame, opacity, D2D.BitmapInterpolationMode.Linear); 
        }
    }
}
