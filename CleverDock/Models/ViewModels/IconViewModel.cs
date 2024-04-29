using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CleverDock.Interop;
using CleverDock.Managers;
using CleverDock.Models;
using CleverDock.Models.ViewModels;
using CleverDock.Patterns;
using CleverDock.Tools;

namespace CleverDock.ViewModels;

public class IconViewModel : ViewModelBase
{
    private const string ThemeFolder = "Themes";
    private readonly IconModel model = new();

    public IconViewModel()
    {
        WindowManager.Manager.ActiveWindowChanged += Manager_ActiveWindowChanged;
        Windows = new ObservableCollection<Win32Window>();
        Windows.CollectionChanged += Windows_CollectionChanged;
        MinimizeCommand = new ActionCommand(MinimizeAction);
        RestoreCommand = new ActionCommand(RestoreAction);
        CloseCommand = new ActionCommand(CloseAction);
        ExitCommand = new ActionCommand(ExitAction);
    }

    public IconViewModel(IconModel model)
        : this()
    {
        Pinned = true;
        Path = model.Path;
        ImagePath = model.ImagePath;
        Name = model.Name;
        Text = StringUtils.LimitCharacters(model.Name, 40, 50);
    }

    #region Utilities

    /// <summary>
    ///     FrameworkElement reference set by DockIconLoadedBehavior.
    /// </summary>
    public FrameworkElement Element { get; set; }

    #endregion

    private void Manager_ActiveWindowChanged(object sender, EventArgs e)
    {
        UpdateWindowState();
    }

    private void UpdateWindowState()
    {
        var window = Windows.FirstOrDefault();
        if (window == null)
            return;
        CanMinimize = !window.IsMinimized;
        CanRestore = window.IsMinimized;
    }

    #region Events

    public event EventHandler OnAnimateIconBounce;

    public void AnimateIconBounce()
    {
        if (OnAnimateIconBounce != null)
            OnAnimateIconBounce(this, null);
    }

    public event EventHandler OnMinimize;

    public void Minimize()
    {
        if (OnMinimize != null)
            OnMinimize(this, null);
        var window = Windows.FirstOrDefault();
        if (window != null)
            window.Minimize();
    }

    public event EventHandler OnRestore;

    public void Restore()
    {
        if (OnRestore != null)
            OnRestore(this, null);
        var window = Windows.FirstOrDefault();
        if (window != null)
            window.Restore();
    }

    public event EventHandler OnClose;

    public void Close()
    {
        if (OnClose != null)
            OnClose(this, null);
        var window = Windows.FirstOrDefault();
        if (window != null)
            window.Close();
    }

    public event EventHandler OnToggle;

    public void Toggle()
    {
        if (OnToggle != null)
            OnToggle(this, null);
        var window = Windows.FirstOrDefault();
        if (window != null)
        {
            if (window.IsActive)
                Minimize();
            else
                Restore();
        }
    }

    #endregion

    #region Methods

    public ActionCommand ExitCommand { get; private set; }

    private void ExitAction()
    {
        Application.Current.Shutdown();
    }

    public ActionCommand MinimizeCommand { get; private set; }

    private void MinimizeAction()
    {
        Minimize();
    }

    public ActionCommand RestoreCommand { get; private set; }

    private void RestoreAction()
    {
        Restore();
    }

    public ActionCommand CloseCommand { get; private set; }

    private void CloseAction()
    {
        Close();
    }

    public void Run()
    {
        if (Windows.Count > 0
            && !Keyboard.IsKeyDown(Key.LeftShift))
        {
            Toggle();
        }
        else if (!string.IsNullOrEmpty(Path) && File.Exists(Path))
        {
            Process.Start(Path);
            AnimateIconBounce();
        }
    }

    private void Windows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateImages();
        UpdateWindowState();
    }

    private void UpdateImages()
    {
        var hasWindows = Windows.Any();
        IsActive = hasWindows;
        if (hasWindows)
        {
            var window = Windows.First();
            Text = string.IsNullOrEmpty(window.Title) ? System.IO.Path.GetFileName(window.FileName) : window.Title;
            Text = StringUtils.LimitCharacters(Text, 40, 50);
            var bitmap = IconManager.GetIcon(window.FileName, VMLocator.Main.IconSize) ?? IconManager.UnknownIcon;
            Icon = bitmap;
            BlurredIcon = BitmapEffectHelper.GaussianBlur(bitmap, 2.5f);
            ChildIcon = IconManager.GetAppIcon(window.Hwnd);
            HasChildIcon = WindowManager.Manager.Windows.Count(w => w.FileName == window.FileName) > 1;
        }

        if (Pinned && string.IsNullOrEmpty(ImagePath))
        {
            var bitmap = IconManager.GetIcon(Path, VMLocator.Main.IconSize) ?? IconManager.UnknownIcon;
            Icon = bitmap;
            BlurredIcon = BitmapEffectHelper.GaussianBlur(bitmap, 2.5f);
        }
    }

    #endregion

    #region Binded Properties

    private ImageSource _icon;

    /// <summary>
    ///     Image of the icon.
    /// </summary>
    public ImageSource Icon
    {
        get => _icon;
        set
        {
            if (value != _icon)
            {
                _icon = value;
                OnPropertyChanged();
            }
        }
    }

    private ImageSource _blurredIcon;

    /// <summary>
    ///     Preblurred image of the icon.
    ///     TODO: Blur the icon in Icon's setter.
    /// </summary>
    public ImageSource BlurredIcon
    {
        get => _blurredIcon;
        set
        {
            if (value != _blurredIcon)
            {
                _blurredIcon = value;
                OnPropertyChanged();
            }
        }
    }

    private ImageSource _childIcon;

    /// <summary>
    ///     Child icon.
    /// </summary>
    public ImageSource ChildIcon
    {
        get => _childIcon;
        set
        {
            if (value != _childIcon)
            {
                _childIcon = value;
                OnPropertyChanged();
            }
        }
    }

    private string _text;

    /// <summary>
    ///     Title of the icon.
    /// </summary>
    public string Text
    {
        get => _text;
        set
        {
            if (value != _text)
            {
                _text = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _isActive;

    /// <summary>
    ///     True if the window is focused.
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (value != _isActive)
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _canMinimize;

    /// <summary>
    ///     True if the window can be minimized
    /// </summary>
    public bool CanMinimize
    {
        get => _canMinimize;
        set
        {
            if (value != _canMinimize)
            {
                _canMinimize = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _canRestore;

    /// <summary>
    ///     True if the window can be restored
    /// </summary>
    public bool CanRestore
    {
        get => _canRestore;
        set
        {
            if (value != _canRestore)
            {
                _canRestore = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _hasChildIcon;

    /// <summary>
    ///     True if the child icon should be displayed
    /// </summary>
    public bool HasChildIcon
    {
        get => _hasChildIcon;
        set
        {
            if (value != _hasChildIcon)
            {
                _hasChildIcon = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<Win32Window> _windows;

    /// <summary>
    ///     Windows attached to this icon.
    /// </summary>
    public ObservableCollection<Win32Window> Windows
    {
        get => _windows;
        set
        {
            if (value != _windows)
            {
                _windows = value;
                OnPropertyChanged();
            }
        }
    }


    private ObservableCollection<ThemeViewModel> _themes;

    /// <summary>
    ///     Available themes.
    /// </summary>
    public ObservableCollection<ThemeViewModel> Themes
    {
        get
        {
            if (_themes == null)
                _themes = new ObservableCollection<ThemeViewModel>
                (new ThemeViewModel[] { new(ThemeManager.DefaultTheme) }
                    .Concat(Directory.GetFiles(ThemeFolder, "theme.json", SearchOption.AllDirectories)
                        .Select(f => new ThemeViewModel(new ThemeModel(f)))));
            return _themes;
        }
    }

    /// <summary>
    ///     The path to the icon's executable or file.
    /// </summary>
    public string Path
    {
        get => model.Path;
        set
        {
            if (value != model.Path)
            {
                model.Path = value;
                UpdateImages();
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Optional. The path to the image used for the icon, if set to null the system icon will be used.
    /// </summary>
    public string ImagePath
    {
        get => model.ImagePath;
        set
        {
            if (value != model.ImagePath)
            {
                model.ImagePath = value;
                UpdateImages();
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The name of the icon.
    /// </summary>
    public string Name
    {
        get => model.Name;
        set
        {
            if (value != model.Name)
            {
                model.Name = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _pinned { get; set; }

    /// <summary>
    ///     When the icon is pinned, it stays in the dock.
    /// </summary>
    public bool Pinned
    {
        get => _pinned;
        set
        {
            if (value != _pinned)
            {
                _pinned = value;
                OnPropertyChanged();
            }
        }
    }

    #endregion
}