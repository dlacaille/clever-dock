using Kaliko.ImageLibrary;
using Kaliko.ImageLibrary.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using WIC = SharpDX.WIC;

namespace CleverDock.Tools
{
    public class BitmapEffectHelper
    {
        public static WIC.BitmapSource GaussianBlur(WIC.BitmapSource image, float radius)
        {
            GaussianBlurFilter filter = new GaussianBlurFilter(radius);
            var factory = new WIC.ImagingFactory();
            using (var kimage = BitmapSourceToKaliko(factory, image))
            {
                kimage.ApplyFilter(filter);
                return KalikoToBitmap(factory, kimage);
            }
        }

        private static KalikoImage BitmapSourceToKaliko(WIC.ImagingFactory factory, WIC.BitmapSource image)
        {
            var stream = new MemoryStream();

            // Encode image using WIC in stream.
            var wicStream = new WIC.WICStream(factory, stream);
            var encoder = new WIC.PngBitmapEncoder(factory);
            encoder.Initialize(wicStream);
            var frameEncoder = new WIC.BitmapFrameEncode(encoder);
            frameEncoder.Initialize();
            frameEncoder.SetSize(image.Size.Width, image.Size.Height);
            var format = WIC.PixelFormat.FormatDontCare;
            frameEncoder.SetPixelFormat(ref format);
            frameEncoder.WriteSource(image);
            frameEncoder.Commit();
            encoder.Commit();

            // Clean up
            frameEncoder.Dispose();
            encoder.Dispose();
            wicStream.Dispose();

            // Return kaliko image.
            stream.Position = 0;
            return new KalikoImage(stream);
        }

        private static WIC.BitmapSource KalikoToBitmap(WIC.ImagingFactory factory, KalikoImage kimage)
        {
            // Save kaliko image to stream.
            var stream = new MemoryStream();
            kimage.SavePng(stream);
            stream.Position = 0;

            // Decode BitmapSource from stream.
            var decoder = new WIC.BitmapDecoder(factory, stream, WIC.DecodeOptions.CacheOnLoad);
            return new WIC.BitmapSource(decoder.GetFrame(0).NativePointer);
        }
    }
}
