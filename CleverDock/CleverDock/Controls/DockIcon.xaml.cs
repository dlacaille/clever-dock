using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CleverDock.Managers;
using CleverDock.Model;
using Window = CleverDock.Model.Window;
using System.Collections.ObjectModel;
using CleverDock.Interop;
using System.Windows.Input;
using CleverDock.Tools;

namespace CleverDock.Controls
{
    /// <summary>
    ///   Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DockIcon : UserControl
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(DockIcon));
        public static readonly DependencyProperty BlurredIconProperty = DependencyProperty.Register("BlurredIcon", typeof(ImageSource), typeof(DockIcon));
        public static readonly DependencyProperty ChildIconProperty = DependencyProperty.Register("ChildIcon", typeof(ImageSource), typeof(DockIcon));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof (string), typeof (DockIcon));
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(DockIcon));
        public ObservableCollection<Window> Windows = new ObservableCollection<Window>();

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public ImageSource BlurredIcon
        {
            get { return (ImageSource)GetValue(BlurredIconProperty); }
            set { SetValue(BlurredIconProperty, value); }
        }

        public ImageSource ChildIcon
        {
            get { return (ImageSource)GetValue(ChildIconProperty); }
            set { SetValue(ChildIconProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public IconInfo Info { get; set; }

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
                    var bitmap = IconManager.GetIcon(info.Path, SettingsManager.Settings.IconSize);
                    Icon = bitmap;
                    BlurredIcon = BitmapEffectHelper.GaussianBlur(bitmap, 2.5f);
                }
                Text = info.Name;
                MenuPin.IsChecked = info.Pinned;
            }
        }

        public DockIcon()
        {
            WindowManager.Manager.ActiveWindowChanged += Manager_ActiveWindowChanged;
            Windows.CollectionChanged += Windows_CollectionChanged;
            ContextMenuOpening += DockIcon_ContextMenuOpening;
            ContextMenuClosing += DockIcon_ContextMenuClosing;
            InitializeComponent();
            IconLight.Visibility = Windows.Any() ? Visibility.Visible : Visibility.Hidden;
            MenuMinimize.IsEnabled = MenuRestore.IsEnabled = MenuClose.IsEnabled = Windows.Any();
            ReserveScreen.IsChecked = SettingsManager.Settings.RemoveTaskbar && SettingsManager.Settings.ReserveScreenSpace;
            ReserveScreen.Click += ReserveScreen_Click;
            RemoveTaskbar.IsChecked = SettingsManager.Settings.RemoveTaskbar;
            RemoveTaskbar.Click += RemoveTaskbar_Click;
            AutoHide.IsChecked = SettingsManager.Settings.AutoHide;
            AutoHide.Click += AutoHide_Click;
            SetDimensions();
            BindThemes();
            SettingsManager.Settings.PropertyChanged += Settings_PropertyChanged;
            ThemeManager.Manager.ThemeChanged += Manager_ThemeChanged;
            ThemeManager.Manager.ThemeListChanged += Manager_ThemeListChanged;
        }

        void DockIcon_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            MainWindow.Window.ContextMenuOpened = false;
        }

        void DockIcon_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            MainWindow.Window.ContextMenuOpened = true;
        }

        void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "ReserveScreenSpace":
                    ReserveScreen.IsChecked = SettingsManager.Settings.ReserveScreenSpace;
                    break;
                case "AutoHide":
                    AutoHide.IsChecked = SettingsManager.Settings.AutoHide;
                    break;
                case "RemoveTaskbar":
                    RemoveTaskbar.IsChecked = SettingsManager.Settings.RemoveTaskbar;
                    break;
            }
        }

        void Manager_ThemeListChanged(object sender, EventArgs e)
        {
            BindThemes();
        }
        
        void AutoHide_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.Settings.AutoHide = !SettingsManager.Settings.AutoHide;
            AutoHide.IsChecked = SettingsManager.Settings.AutoHide;
        }

        void ReserveScreen_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.Settings.ReserveScreenSpace = !SettingsManager.Settings.ReserveScreenSpace;
            ReserveScreen.IsChecked = SettingsManager.Settings.ReserveScreenSpace;
        }

        void RemoveTaskbar_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.Settings.RemoveTaskbar = !SettingsManager.Settings.RemoveTaskbar;
            RemoveTaskbar.IsChecked = SettingsManager.Settings.RemoveTaskbar;
        }

        void Manager_ThemeChanged(object sender, Handlers.ThemeEventArgs e)
        {
            BindThemes();
        }

        private void BindThemes()
        {
            SelectTheme.Items.Clear();
            foreach(var theme in ThemeManager.Manager.GetThemes())
            {
                var menuItem = new MenuItem();
                menuItem.Header = theme.Name;
                menuItem.Tag = theme;
                menuItem.IsChecked = SettingsManager.Settings.Theme.Path == theme.Path;
                menuItem.Click += menuItem_Click;
                SelectTheme.Items.Add(menuItem);
            }
        }

        void menuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            ThemeManager.Manager.LoadTheme(menuItem.Tag as Theme);
        }

        public DockIconContainer GetContainer()
        {
            return (DockIconContainer)Parent;
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        void Windows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            bool hasWindows = Windows.Any();

            IconLight.Visibility = hasWindows ? Visibility.Visible : Visibility.Hidden;
            MenuMinimize.IsEnabled = MenuRestore.IsEnabled = MenuClose.IsEnabled = hasWindows;

            if (hasWindows)
            {
                Window window = Windows.First();
                Text = StringUtils.LimitCharacters(window.Title, 40, 50);
                var bitmap = IconManager.GetIcon(window.FileName, SettingsManager.Settings.IconSize);
                Icon = bitmap;
                BlurredIcon = BitmapEffectHelper.GaussianBlur(bitmap, 2.5f);
                ChildIcon = IconManager.GetAppIcon(window.Hwnd);
                if (WindowManager.Manager.Windows.Count(w => w.FileName == window.FileName) > 1)
                    IconImageSmall.Visibility = Visibility.Visible;
            }
            if (Info.Pinned && string.IsNullOrEmpty(Info.ImagePath))
            {
                var bitmap = IconManager.GetIcon(Info.Path, SettingsManager.Settings.IconSize);
                Icon = bitmap;
                BlurredIcon = BitmapEffectHelper.GaussianBlur(bitmap, 2.5f);
            }
        }

        void Manager_ActiveWindowChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                IsActive = Windows.Any(w => w.Hwnd == (IntPtr) sender);
            });
        }

        public void SetDimensions()
        {
            Width = SettingsManager.Settings.OuterIconWidth;
            Height = SettingsManager.Settings.OuterIconHeight;
            IconImage.Width = IconImage.Height = SettingsManager.Settings.IconSize;
        }

        public void Run()
        {
            if (Windows.Count > 0 
                && !Keyboard.IsKeyDown(Key.LeftShift))
                Windows[0].Toggle();
            else if (Info != null && !string.IsNullOrEmpty(Info.Path) && File.Exists(Info.Path))
            {
                Process.Start(Info.Path);
                AnimateIconBounce();
            }
        }

        public void AnimateIconBounce()
        {
            var translation = new DoubleAnimation
            {
                    From = 0,
                    To = -30,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true
            };
            translation.EasingFunction = new BounceEase
            {
                    Bounces = 1,
                    Bounciness = 1,
                    EasingMode = EasingMode.EaseIn
            };
            Storyboard.SetTargetName(translation, "ImageTransform");
            Storyboard.SetTargetProperty(translation, new PropertyPath(TranslateTransform.YProperty));

            var sb = new Storyboard();
            sb.Children.Add(translation);
            sb.Begin(IconGrid);
        }

        private void MenuPin_Click(object sender, RoutedEventArgs e)
        {
            MenuPin.IsChecked = Info.Pinned = !Info.Pinned;
            if (!Info.Pinned && !Windows.Any())
                GetContainer().Children.Remove(this);
        }

        private void MenuMinimize_Click(object sender, RoutedEventArgs e)
        {
            Windows.First().Minimize();
        }

        private void MenuRestore_Click(object sender, RoutedEventArgs e)
        {
            Windows.First().Restore();
        }

        private void MenuClose_Click(object sender, RoutedEventArgs e)
        {
            Windows.First().Close();
        }

        private void MenuCloseDock_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}