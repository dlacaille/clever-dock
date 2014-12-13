using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WI = CleverDock.Interop.WindowInterop;
using II = CleverDock.Interop.IconInterop;

namespace CleverDock.Managers
{
    public class IconManager
    {
        private static BitmapSource IconSource(IntPtr handle)
        {
            var result = Imaging.CreateBitmapSourceFromHIcon(handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            result.Freeze();
            return result;
        }

        public static BitmapSource GetAppIcon(IntPtr hwnd)
        {
            IntPtr hIcon = WI.GetClassLongPtr(hwnd, WI.ICON_SMALL);
            try
            {
                if (hIcon == IntPtr.Zero)
                    hIcon = WI.SendMessage(hwnd, WI.WM_GETICON, WI.ICON_SMALL2, 0);
                if (hIcon == IntPtr.Zero)
                    hIcon = WI.SendMessage(hwnd, WI.WM_GETICON, WI.ICON_BIG, 0);
                if (hIcon == IntPtr.Zero)
                    hIcon = WI.GetClassLongPtr(hwnd, WI.GCL_HICON);
                if (hIcon == IntPtr.Zero)
                    hIcon = WI.GetClassLongPtr(hwnd, WI.GCL_HICONSM);
            }
            catch (Exception)
            { }
            if (hIcon == IntPtr.Zero)
                return null;
            var bs = IconSource(hIcon);
            return bs;
        }

        public static BitmapSource GetSmallIcon(string FileName, bool small)
        {
            var shinfo = new II.SHFILEINFO();
            uint flags;

            if (small)
                flags = II.SHGFI_ICON | II.SHGFI_SMALLICON;
            else
                flags = II.SHGFI_ICON | II.SHGFI_LARGEICON;

            var res = II.SHGetFileInfo(FileName, 0, ref shinfo, Marshal.SizeOf(shinfo), flags);

            if (res == 0)
                throw (new FileNotFoundException());

            var bs = IconSource(shinfo.hIcon);

            bs.Freeze(); // very important to avoid memory leak
            II.DestroyIcon(shinfo.hIcon);

            return bs;
        }

        public static BitmapSource GetIcon(string path, int size)
        {
            if (size <= 16)
                return GetSmallIcon(path, true);
            if (size <= 32)
                return GetSmallIcon(path, false);
            if (size <= 48)
                return GetLargeIcon(path, false);
            return GetLargeIcon(path, true);
        }

        public static BitmapSource GetLargeIcon(string path, bool jumbo)
        {
            var shinfo = new II.SHFILEINFO();
            const uint SHGFI_SYSICONINDEX = 0x4000;
            const int FILE_ATTRIBUTE_NORMAL = 0x80;
            var flags = SHGFI_SYSICONINDEX;

            var res = II.SHGetFileInfo(path, FILE_ATTRIBUTE_NORMAL, ref shinfo, Marshal.SizeOf(shinfo), flags);

            if (res == 0)
                throw (new FileNotFoundException());

            var iconIndex = shinfo.iIcon;

            // Get the System IImageList object from the Shell:
            var iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");

            II.IImageList iml;
            var size = jumbo ? II.SHIL_JUMBO : II.SHIL_EXTRALARGE;
            II.SHGetImageList(size, ref iidImageList, out iml);
            var hIcon = IntPtr.Zero;
            const int ILD_TRANSPARENT = 1;
            iml.GetIcon(iconIndex, ILD_TRANSPARENT, ref hIcon);

            var bs = IconSource(hIcon);

            bs.Freeze(); // very important to avoid memory leak
            II.DestroyIcon(hIcon);
            II.SendMessage(hIcon, II.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            return bs;
        }
    }
}