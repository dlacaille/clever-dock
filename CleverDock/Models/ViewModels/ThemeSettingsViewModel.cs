using CleverDock.Models;
using CleverDock.Patterns;

namespace CleverDock.ViewModels;

public class ThemeSettingsViewModel : ViewModelBase
{
    public static ThemeSettingsViewModel DefaultThemeSettings = new()
    {
        IconPaddingX = 15,
        IconPaddingY = 20
    };

    private string _author;

    private string _description;

    private int _iconPaddingX;

    private int _iconPaddingY;

    private string _themeName;

    private string _version;

    public ThemeSettingsViewModel()
    {
    }

    public ThemeSettingsViewModel(ThemeSettingsModel model)
    {
        ThemeName = model.ThemeName;
        Author = model.Author;
        Version = model.Version;
        Description = model.Description;
        IconPaddingX = model.IconPaddingX;
        IconPaddingY = model.IconPaddingY;
    }

    /// <summary>
    ///     Name of the Theme.
    /// </summary>
    public string ThemeName
    {
        get => _themeName;
        set
        {
            if (value != _themeName)
            {
                _themeName = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Author of the Theme.
    /// </summary>
    public string Author
    {
        get => _author;
        set
        {
            if (value != _author)
            {
                _author = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Version of the Theme.
    /// </summary>
    public string Version
    {
        get => _version;
        set
        {
            if (value != _version)
            {
                _version = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Description of the Theme.
    /// </summary>
    public string Description
    {
        get => _description;
        set
        {
            if (value != _description)
            {
                _description = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     X icon padding.
    /// </summary>
    public int IconPaddingX
    {
        get => _iconPaddingX;
        set
        {
            if (value != _iconPaddingX)
            {
                _iconPaddingX = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     Y icon padding
    /// </summary>
    public int IconPaddingY
    {
        get => _iconPaddingY;
        set
        {
            if (value != _iconPaddingY)
            {
                _iconPaddingY = value;
                OnPropertyChanged();
            }
        }
    }
}