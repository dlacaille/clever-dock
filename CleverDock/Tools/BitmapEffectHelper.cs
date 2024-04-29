using System.IO;
using System.Windows.Media.Imaging;
using Kaliko.ImageLibrary;
using Kaliko.ImageLibrary.Filters;

namespace CleverDock.Tools;

public class BitmapEffectHelper
{
    public static BitmapImage GaussianBlur(BitmapSource image, float radius)
    {
        var filter = new GaussianBlurFilter(radius);
        using (var kimage = BitmapSourceToKaliko(image))
        {
            if (kimage == null)
                return null;
            var padding = (int)Math.Ceiling(radius);
            kimage.Crop(-padding, padding, (int)image.Width + padding * 2, (int)image.Height + padding * 2);
            kimage.ApplyFilter(filter);
            return KalikoToBitmap(kimage);
        }
    }

    private static KalikoImage BitmapSourceToKaliko(BitmapSource image)
    {
        if (image == null)
            return null;
        var stream = new MemoryStream();
        BitmapEncoder enc = new PngBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(image));
        enc.Save(stream);
        stream.Position = 0;
        return new KalikoImage(stream);
    }

    private static BitmapImage KalikoToBitmap(KalikoImage kimage)
    {
        var stream = new MemoryStream();
        kimage.SavePng(stream);
        stream.Position = 0;
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = stream;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }
}