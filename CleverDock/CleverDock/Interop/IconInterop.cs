using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CleverDock.Interop
{
    internal class IconInterop
    {
        /// <summary>
        ///   Retrieve the handle to the icon that represents the file and the index of the icon within the system image list. The handle is copied to the hIcon member of the structure specified by psfi, and the index is copied to the iIcon member.
        /// </summary>
        public const int SHGFI_ICON = 0x100;

        /// <summary>
        ///   Modify SHGFI_ICON, causing the function to retrieve the file's large icon (32x32). The SHGFI_ICON flag must also be set.
        /// </summary>
        public const int SHGFI_LARGEICON = 0x0;

        /// <summary>
        ///   Modify SHGFI_ICON, causing the function to retrieve the file's small icon (16x16). The SHGFI_ICON flag must also be set.
        /// </summary>
        public const int SHGFI_SMALLICON = 0x1;

        /// <summary>
        ///   Used by SHGetImageList. These images are the Shell standard extra-large icon size. This is typically 48x48, but the size can be customized by the user.
        /// </summary>
        public const int SHIL_EXTRALARGE = 0x2;

        /// <summary>
        ///   Used by SHGetImageList. Windows Vista and later. The image is normally 256x256 pixels.
        /// </summary>
        public const int SHIL_JUMBO = 0x4;

        public const int WM_CLOSE = 0x0010;

        /// <summary>
        ///   Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
        /// </summary>
        /// <param name="handle"> A handle to the window whose window procedure will receive the message. </param>
        /// <param name="Msg"> The message to be sent. </param>
        /// <param name="wParam"> Additional message-specific information. </param>
        /// <param name="lParam"> Additional message-specific information. </param>
        /// <returns> The return value specifies the result of the message processing; it depends on the message sent. </returns>
        [DllImport("user32")]
        public static extern
        IntPtr SendMessage(
                IntPtr handle,
                int Msg,
                IntPtr wParam,
                IntPtr lParam
                );

        /// SHGetImageList is not exported correctly in XP.  See KB316931
        /// http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316931
        /// Apparently (and hopefully) ordinal 727 isn't going to change.
        /// <summary>
        ///   Retrieves an image list.
        /// </summary>
        /// <param name="iImageList"> The image type contained in the list. </param>
        /// <param name="riid"> Reference to the image list interface identifier, normally IID_IImageList. </param>
        /// <param name="ppv"> When this method returns, contains the interface pointer requested in riid. This is typically IImageList. </param>
        /// <returns> If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code. </returns>
        [DllImport("shell32.dll", EntryPoint = "#727")]
        public static extern int SHGetImageList(
                int iImageList,
                ref Guid riid,
                out IImageList ppv
                );

        /// <summary>
        ///   Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.
        /// </summary>
        /// <param name="pszPath"> A pointer to a null-terminated string of maximum length MAX_PATH that contains the path and file name. Both absolute and relative paths are valid. </param>
        /// <param name="dwFileAttributes"> A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored. </param>
        /// <param name="psfi"> Pointer to a SHFILEINFO structure to receive the file information. </param>
        /// <param name="cbFileInfo"> The size, in bytes, of the SHFILEINFO structure pointed to by the psfi parameter. </param>
        /// <param name="uFlags"> The flags that specify the file information to retrieve. </param>
        /// <returns> Returns a value whose meaning depends on the uFlags parameter. </returns>
        [DllImport("Shell32.dll")]
        public static extern int SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo,
                                               uint uFlags);

        /// <summary>
        ///   Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <param name="hIcon"> A handle to the icon to be destroyed. The icon must not be in use. </param>
        /// <returns> If the function succeeds, the return value is nonzero. </returns>
        [DllImport("user32")]
        public static extern int DestroyIcon(IntPtr hIcon);

        #region Nested type: IMAGEINFO

        /// <summary>
        ///   Contains information about an image in an image list. This structure is used with the IImageList::GetImageInfo function.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGEINFO
        {
            /// <summary>
            ///   A handle to the bitmap that contains the images.
            /// </summary>
            private readonly IntPtr hbmImage;

            /// <summary>
            ///   A handle to a monochrome bitmap that contains the masks for the images. If the image list does not contain a mask, this member is NULL.
            /// </summary>
            private readonly IntPtr hbmMask;

            /// <summary>
            ///   Not used. This member should always be zero.
            /// </summary>
            private readonly int Unused1;

            /// <summary>
            ///   Not used. This member should always be zero.
            /// </summary>
            private readonly int Unused2;

            /// <summary>
            ///   The bounding rectangle of the specified image within the bitmap specified by hbmImage.
            /// </summary>
            private readonly RECT rcImage;
        }

        #endregion

        #region Private ImageList COM Interop (XP)

        /// <summary>
        ///   Exposes methods that manipulate and interact with image lists.
        /// </summary>
        [ComImport]
        [Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IImageList
        {
            [PreserveSig]
            int Add(
                    IntPtr hbmImage,
                    IntPtr hbmMask,
                    ref int pi);

            [PreserveSig]
            int ReplaceIcon(
                    int i,
                    IntPtr hicon,
                    ref int pi);

            [PreserveSig]
            int SetOverlayImage(
                    int iImage,
                    int iOverlay);

            [PreserveSig]
            int Replace(
                    int i,
                    IntPtr hbmImage,
                    IntPtr hbmMask);

            [PreserveSig]
            int AddMasked(
                    IntPtr hbmImage,
                    int crMask,
                    ref int pi);

            [PreserveSig]
            int Draw(
                    ref IMAGELISTDRAWPARAMS pimldp);

            [PreserveSig]
            int Remove(
                    int i);

            [PreserveSig]
            int GetIcon(
                    int i,
                    int flags,
                    ref IntPtr picon);

            [PreserveSig]
            int GetImageInfo(
                    int i,
                    ref IMAGEINFO pImageInfo);

            [PreserveSig]
            int Copy(
                    int iDst,
                    IImageList punkSrc,
                    int iSrc,
                    int uFlags);

            [PreserveSig]
            int Merge(
                    int i1,
                    IImageList punk2,
                    int i2,
                    int dx,
                    int dy,
                    ref Guid riid,
                    ref IntPtr ppv);

            [PreserveSig]
            int Clone(
                    ref Guid riid,
                    ref IntPtr ppv);

            [PreserveSig]
            int GetImageRect(
                    int i,
                    ref RECT prc);

            [PreserveSig]
            int GetIconSize(
                    ref int cx,
                    ref int cy);

            [PreserveSig]
            int SetIconSize(
                    int cx,
                    int cy);

            [PreserveSig]
            int GetImageCount(
                    ref int pi);

            [PreserveSig]
            int SetImageCount(
                    int uNewCount);

            [PreserveSig]
            int SetBkColor(
                    int clrBk,
                    ref int pclr);

            [PreserveSig]
            int GetBkColor(
                    ref int pclr);

            [PreserveSig]
            int BeginDrag(
                    int iTrack,
                    int dxHotspot,
                    int dyHotspot);

            [PreserveSig]
            int EndDrag();

            [PreserveSig]
            int DragEnter(
                    IntPtr hwndLock,
                    int x,
                    int y);

            [PreserveSig]
            int DragLeave(
                    IntPtr hwndLock);

            [PreserveSig]
            int DragMove(
                    int x,
                    int y);

            [PreserveSig]
            int SetDragCursorImage(
                    ref IImageList punk,
                    int iDrag,
                    int dxHotspot,
                    int dyHotspot);

            [PreserveSig]
            int DragShowNolock(
                    int fShow);

            [PreserveSig]
            int GetDragImage(
                    ref POINT ppt,
                    ref POINT pptHotspot,
                    ref Guid riid,
                    ref IntPtr ppv);

            [PreserveSig]
            int GetItemFlags(
                    int i,
                    ref int dwFlags);

            [PreserveSig]
            int GetOverlayImage(
                    int iOverlay,
                    ref int piIndex);
        };

        #endregion

        #region Nested type: IMAGELISTDRAWPARAMS

        /// <summary>
        ///   Contains information about an image list draw operation and is used with the IImageList::Draw function.
        /// </summary>
        public struct IMAGELISTDRAWPARAMS
        {
            /// <summary>
            ///   Used with the alpha blending effect.
            ///   When used with ILS_ALPHA, this member holds the value for the alpha channel. This value can be from 0 to 255, with 0 being completely transparent, and 255 being completely opaque.
            ///   You must use comctl32.dll version 6 to use this member. See the Remarks.
            /// </summary>
            public int Frame;

            /// <summary>
            ///   The size of this structure, in bytes.
            /// </summary>
            public int cbSize;

            /// <summary>
            ///   A color used for the glow and shadow effects. You must use comctl32.dll version 6 to use this member. See the Remarks.
            /// </summary>
            public int crEffect;

            /// <summary>
            ///   A value that specifies the number of pixels to draw, relative to the upper-left corner of the drawing operation as specified by xBitmap and yBitmap. If cx and cy are zero, then Draw draws the entire valid section. The method does not ensure that the parameters are valid.
            /// </summary>
            public int cx;

            /// <summary>
            ///   A value that specifies the number of pixels to draw, relative to the upper-left corner of the drawing operation as specified by xBitmap and yBitmap. If cx and cy are zero, then Draw draws the entire valid section. The method does not ensure that the parameters are valid.
            /// </summary>
            public int cy;

            /// <summary>
            ///   A value specifying a raster operation code. These codes define how the color data for the source rectangle will be combined with the color data for the destination rectangle to achieve the final color. This member is ignored if fStyle does not include the ILD_ROP flag.
            /// </summary>
            public int dwRop;

            /// <summary>
            ///   A flag that specifies the drawing state. This member can contain one or more image list state flags. You must use comctl32.dll version 6 to use this member.
            /// </summary>
            public int fState;

            /// <summary>
            ///   A flag specifying the drawing style and, optionally, the overlay image. See the comments section at the end of this topic for information on the overlay image.
            /// </summary>
            public int fStyle;

            /// <summary>
            ///   A handle to the destination device context.
            /// </summary>
            public IntPtr hdcDst;

            /// <summary>
            ///   A handle to the image list that contains the image to be drawn.
            /// </summary>
            public IntPtr himl;

            /// <summary>
            ///   The zero-based index of the image to be drawn.
            /// </summary>
            public int i;

            /// <summary>
            ///   The image background color.
            /// </summary>
            public int rgbBk;

            /// <summary>
            ///   The image foreground color. This member is used only if fStyle includes the ILD_BLEND25 or ILD_BLEND50 flag.
            /// </summary>
            public int rgbFg;

            /// <summary>
            ///   The x-coordinate that specifies where the image is drawn.
            /// </summary>
            public int x;

            /// <summary>
            ///   The x-coordinate that specifies the upper-left corner of the drawing operation in reference to the image itself. Pixels of the image that are to the left of xBitmap and above yBitmap do not appear.
            /// </summary>
            public int xBitmap;

            /// <summary>
            ///   The y-coordinate that specifies where the image is drawn.
            /// </summary>
            public int y;

            /// <summary>
            ///   The y-coordinate that specifies the upper-left corner of the drawing operation in reference to the image itself. Pixels of the image that are to the left of xBitmap and above yBitmap do not appear.
            /// </summary>
            public int yBitmap;
        }

        #endregion

        #region Nested type: POINT

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static implicit operator Point(POINT p)
            {
                return new Point(p.X, p.Y);
            }

            public static implicit operator POINT(Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        #endregion

        #region Nested type: RECT

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            private readonly int _Left;
            private readonly int _Top;
            private readonly int _Right;
            private readonly int _Bottom;
        }

        #endregion

        #region Nested type: SHFILEINFO

        /// <summary>
        ///   Contains information about a file object.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            /// <summary>
            ///   A handle to the icon that represents the file.
            /// </summary>
            public IntPtr hIcon;

            /// <summary>
            ///   The index of the icon image within the system image list.
            /// </summary>
            public int iIcon;

            /// <summary>
            ///   An array of values that indicates the attributes of the file object.
            /// </summary>
            public uint dwAttributes;

            /// <summary>
            ///   A string that contains the name of the file as it appears in the Windows Shell, or the path and file name of the file that contains the icon representing the file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szDisplayName;

            /// <summary>
            ///   A string that describes the type of file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)] public string szTypeName;
        };

        #endregion
    }
}