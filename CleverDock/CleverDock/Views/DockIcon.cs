using CleverDock.Graphics;
using CleverDock.Graphics.Views;
using CleverDock.Managers;
using CleverDock.Model;
using CleverDock.Tools;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CleverDock.Views
{
    public class DockIcon : View
    {
        private BitmapSource icon;
        public BitmapSource Icon 
        {
            get
            {
                return icon;
            }
            set
            {
                icon = value;
                if (iconView != null)
                    iconView.BitmapSource = value;
            }
        }

        private BitmapSource blurredIcon;
        public BitmapSource BlurredIcon 
        { 
            get
            {
                return blurredIcon;
            }
            set
            {
                blurredIcon = value;
                if (blurredIconView != null)
                    blurredIconView.BitmapSource = value;
            }
        }

        private BitmapSource childIcon { get; set; }
        public BitmapSource ChildIcon
        {
            get
            {
                return childIcon;
            }
            set
            {
                childIcon = value;
                if (childIconView != null)
                    childIconView.BitmapSource = value;
            }
        }

        public IconInfo Info { get; set; }
        public string Text { get; set; }
        public ObservableCollection<Window> Windows = new ObservableCollection<Window>();

        private ImageView iconView;
        private ImageView blurredIconView;
        private ImageView childIconView;

        private TextFormat textFormat;
        private SharpDX.DirectWrite.Factory writeFactory;
        private SolidColorBrush windowIndicatorBrush;
        private SolidColorBrush textBrush;
        private SolidColorBrush textShadowBrush;

        public DockIcon()
        {
            Windows.CollectionChanged += Windows_CollectionChanged;
            Subviews.Add(iconView = new ImageView(new RectangleF(0, 0, 48, 48), Icon));
        }

        public DockIcon(IconInfo info)
            : this()
        {
            Info = info;
            if (!File.Exists(info.Path))
            {
                // TODO: Manage exceptions
            }
            else
            {
                if (string.IsNullOrEmpty(info.ImagePath))
                {
                    var bitmap = IconHelper.GetIcon(info.Path, SettingsManager.Settings.IconSize);
                    Icon = bitmap;
                    BlurredIcon = BitmapEffectHelper.GaussianBlur(bitmap, 2.5f);
                }
                Text = info.Name;
            }
        }

        protected override void OnClick(Point mousePos)
        {
            base.OnClick(mousePos);

            if (Windows.Count > 0
                && !Keyboard.IsKeyDown(Key.LeftShift)) // Always launch if left shift is pressed.
                Windows[0].Toggle();
            else if (Info != null && !string.IsNullOrEmpty(Info.Path) && File.Exists(Info.Path))
            {
                Process.Start(Info.Path);
                //AnimateIconBounce();
            }
        }

        void Windows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            bool hasWindows = Windows.Any();
            // Show icon from window collection.
            if (hasWindows)
            {
                Window window = Windows.First();
                Text = StringUtils.LimitCharacters(window.Title, 50, 60);
                var bitmap = IconHelper.GetIcon(window.FileName, SettingsManager.Settings.IconSize);
                Icon = bitmap;
                BlurredIcon = BitmapEffectHelper.GaussianBlur(bitmap, 2.5f);
                ChildIcon = IconHelper.GetAppIcon(window.Hwnd);
            }
            // Show icon from pinned icon information.
            if (Info.Pinned && string.IsNullOrEmpty(Info.ImagePath))
            {
                var bitmap = IconHelper.GetIcon(Info.Path, SettingsManager.Settings.IconSize);
                Icon = bitmap;
                BlurredIcon = BitmapEffectHelper.GaussianBlur(bitmap, 2.5f);
            }
        }
        
        protected override void OnCreateResources()
        {
            writeFactory = new SharpDX.DirectWrite.Factory();
            textFormat = new TextFormat(writeFactory, "Arial", 14);
            textFormat.TextAlignment = TextAlignment.Center;
            textBrush = new SolidColorBrush(RenderTarget, new Color4(1f, 1f, 1f, 1f));
            textShadowBrush = new SolidColorBrush(RenderTarget, new Color4(0f, 0f, 0f, 0.5f));
            windowIndicatorBrush = new SolidColorBrush(RenderTarget, new Color4(1f, 1f, 1f, 0.7f));

            // Start animation.
            base.OnCreateResources();
        }

        protected override void OnFreeResources()
        {
            // Stop animation.
            base.OnFreeResources();

            if (textShadowBrush != null)
            {
                textShadowBrush.Dispose();
                textShadowBrush = null;
            }
            if (textBrush != null)
            {
                textBrush.Dispose();
                textBrush = null;
            }
            if (windowIndicatorBrush != null)
            {
                windowIndicatorBrush.Dispose();
                windowIndicatorBrush = null;
            }
            if (textFormat != null)
            {
                textFormat.Dispose();
                textFormat = null;
            }
            if (writeFactory != null)
            {
                writeFactory.Dispose();
                writeFactory = null;
            }
        }

        protected override void OnRender()
        {
            base.OnRender();
            if (Windows.Any())
            {
                var radius = 2;
                var ellipseCenter = new Vector2(Frame.Left + Frame.Width / 2, Frame.Bottom + 8);
                var ellipse = new Ellipse(ellipseCenter, radius, radius);
                RenderTarget.FillEllipse(ellipse, windowIndicatorBrush);
            }
            // Draw label if mouse is hovering the icon.
            if (IsMouseOver)
            {
                var maxWidth = 200;
                var labelFrame = new RectangleF(Frame.Left - maxWidth / 2 + Frame.Width / 2, Frame.Top - 60, maxWidth, 20);
                var shadowFrame = labelFrame;
                shadowFrame.Offset(1, 1);
                RenderTarget.DrawText(Text, textFormat, shadowFrame, textShadowBrush);
                RenderTarget.DrawText(Text, textFormat, labelFrame, textBrush);
            }
        }
    }
}
