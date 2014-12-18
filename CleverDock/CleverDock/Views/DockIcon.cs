using CleverDock.Graphics;
using CleverDock.Graphics.Views;
using CleverDock.Managers;
using CleverDock.Model;
using CleverDock.Tools;
using SharpDX;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
