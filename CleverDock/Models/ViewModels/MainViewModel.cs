using System.Collections.ObjectModel;
using System.Windows;
using CleverDock.Managers;
using CleverDock.Models;
using CleverDock.Patterns;

namespace CleverDock.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly SettingsModel settings = new();

    public MainViewModel()
    {
        Application.Current.Exit += Current_Exit;
    }

    private void Current_Exit(object sender, ExitEventArgs e)
    {
        settings.SaveAsFile(SettingsModel.SETTINGS_FILE);
    }

    #region Commands

    #endregion

    #region Properties

    /// <summary>
    ///     Duration time in seconds for the collapsing animation.
    /// </summary>
    public double CollapseDuration
    {
        get => settings.CollapseDuration;
        set
        {
            if (value != settings.CollapseDuration)
            {
                settings.CollapseDuration = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Duration time in seconds for the hiding animation.
    /// </summary>
    public double DockHideDuration
    {
        get => settings.DockHideDuration;
        set
        {
            if (value != settings.DockHideDuration)
            {
                settings.DockHideDuration = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Duration time in seconds for the show animation.
    /// </summary>
    public double DockShowDuration
    {
        get => settings.DockShowDuration;
        set
        {
            if (value != settings.DockShowDuration)
            {
                settings.DockShowDuration = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Seconds the dock should stay visible before hiding.
    /// </summary>
    public double DockHideDelay
    {
        get => settings.DockHideDelay;
        set
        {
            if (value != settings.DockHideDelay)
            {
                settings.DockHideDelay = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Seconds the mouse must stay in the hotspot before showing the dock.
    /// </summary>
    public double DockShowDelay
    {
        get => settings.DockShowDelay;
        set
        {
            if (value != settings.DockShowDelay)
            {
                settings.DockShowDelay = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Height of the hotspot at the bottom of the screen.
    /// </summary>
    public int HotspotHeight
    {
        get => settings.HotspotHeight;
        set
        {
            if (value != settings.HotspotHeight)
            {
                settings.HotspotHeight = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Size of the icons in pixels.
    /// </summary>
    public int IconSize
    {
        get => settings.IconSize;
        set
        {
            if (value != settings.IconSize)
            {
                settings.IconSize = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Theme of the dock.
    /// </summary>
    public ThemeModel Theme
    {
        get => settings.Theme;
        set
        {
            if (value != settings.Theme)
            {
                settings.Theme = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<IconViewModel> _icons;

    /// <summary>
    ///     Icons of the dock.
    /// </summary>
    public ObservableCollection<IconViewModel> Icons
    {
        get
        {
            if (IsInDesignMode)
                _icons = new ObservableCollection<IconViewModel>
                {
                    new(new IconModel { Name = "explorer.exe", Path = @"explorer.exe" }),
                    new(new IconModel { Name = "notepad.exe", Path = @"notepad.exe" }),
                    new(new IconModel { Name = "unknown.exe", Path = @"unknown.exe" })
                };
            if (_icons == null)
                _icons = new ObservableCollection<IconViewModel>(settings.Icons.Select(i => new IconViewModel(i)));
            return _icons;
        }
    }

    /// <summary>
    ///     Save the settings automatically or when the dock is closed.
    /// </summary>
    public bool SaveAutomatically
    {
        get => settings.SaveAutomatically;
        set
        {
            if (value != settings.SaveAutomatically)
            {
                settings.SaveAutomatically = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Reserve the bottom of the screen for the dock.
    /// </summary>
    public bool ReserveScreenSpace
    {
        get => settings.ReserveScreenSpace;
        set
        {
            if (value != settings.ReserveScreenSpace)
            {
                settings.ReserveScreenSpace = value;
                OnPropertyChanged();
            }

            DockManager.Manager.SetWorkingArea(settings.ReserveScreenSpace);
        }
    }

    /// <summary>
    ///     Remote windows taskbar to replace it with the dock.
    /// </summary>
    public bool RemoveTaskbar
    {
        get => settings.RemoveTaskbar;
        set
        {
            if (value != settings.RemoveTaskbar)
            {
                settings.RemoveTaskbar = value;
                OnPropertyChanged();
            }

            TaskbarManager.SetTaskbarVisibility(!value);
        }
    }

    /// <summary>
    ///     Hide the dock automatically when it occludes an other window.
    /// </summary>
    public bool AutoHide
    {
        get => settings.AutoHide;
        set
        {
            if (value != settings.AutoHide)
            {
                settings.AutoHide = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Show the widgets in the corner of the screen.
    /// </summary>
    public bool ShowWidgets
    {
        get => settings.ShowWidgets;
        set
        {
            if (value != settings.ShowWidgets)
            {
                settings.ShowWidgets = value;
                OnPropertyChanged();
            }
        }
    }

    #endregion
}