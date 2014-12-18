using CleverDock.Graphics;
using CleverDock.Managers;
using CleverDock.Model;
using CleverDock.Tools;
using SharpDX;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Views
{
    public class DockIcon : View
    {
        public BitmapSource Icon { get; set; }
        public BitmapSource BlurredIcon { get; set; }
        public BitmapSource ChildIcon { get; set; }

        public IconInfo Info { get; set; }

        public string Text { get; set; }

        public ObservableCollection<Window> Windows = new ObservableCollection<Window>();

        public DockIcon(RectangleF bounds)
            : base(bounds)
        {
            Windows.CollectionChanged += Windows_CollectionChanged;
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
