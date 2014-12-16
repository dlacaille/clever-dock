using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX.WindowsImagingComponent;
using Microsoft.WindowsAPICodePack.DirectX.Graphics;
using System.Drawing;
using CleverDock.Direct2D.Tools;

namespace CleverDock.Direct2D.Views
{
    public class ImageView : View
    {
        private Image image;
        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                this.bitmapSource = BitmapHelper.ImageToBitmapSource(image);
            }
        }

        private BitmapSource bitmapSource { get; set; }
        private D2DBitmap bitmap { get; set; }
        private float opacity { get; set; }

        public ImageView(Rectangle bounds, Image image)
            : base(bounds)
        {
            this.Image = image;
            this.opacity = 1;
        }

        protected override void OnCreateResources()
        {
            var pf = new PixelFormat(Format.B8G8R8A8UNorm, AlphaMode.Premultiplied);
            var properties = new BitmapProperties(pf, (float)bitmapSource.Resolution.DpiX, (float)bitmapSource.Resolution.DpiY);
            bitmap = RenderTarget.CreateBitmapFromWicBitmap(bitmapSource, properties);

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

            RenderTarget.DrawBitmap(bitmap, opacity, BitmapInterpolationMode.Linear, Frame); 
        }
    }
}
