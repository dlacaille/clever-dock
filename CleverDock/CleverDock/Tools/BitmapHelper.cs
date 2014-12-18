using WIC = SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CleverDock.Tools
{
    public class BitmapHelper
    {
        private static WIC.ImagingFactory factory = null;
        private static WIC.ImagingFactory Factory
        {
            get
            {
                if (factory == null)
                    factory = new WIC.ImagingFactory();
                return factory;
            }
        }

        public static WIC.BitmapSource FromImage(Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Position = 0;
            var decoder = new WIC.BitmapDecoder(Factory, stream, WIC.DecodeOptions.CacheOnLoad);
            return new WIC.BitmapSource(decoder.GetFrame(0).NativePointer);
        }
        public static WIC.BitmapSource FromFile(string filename)
        {
            var decoder = new WIC.BitmapDecoder(Factory, filename, WIC.DecodeOptions.CacheOnLoad);
            return new WIC.BitmapSource(decoder.GetFrame(0).NativePointer);
        }
    }
}
