using WIC = Microsoft.WindowsAPICodePack.DirectX.WindowsImagingComponent;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CleverDock.Direct2D.Tools
{
    public class BitmapHelper
    {
        public static WIC.BitmapSource ImageToBitmapSource(Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Position = 0;
            var factory = WIC.ImagingFactory.Create();
            var decoder = factory.CreateDecoderFromStream(stream, WIC.DecodeMetadataCacheOption.OnLoad);
            return decoder.GetFrame(0).ToBitmapSource();
        }
    }
}
