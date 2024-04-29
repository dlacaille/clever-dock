using CleverDock.Managers;
using CleverDock.Patterns;

namespace CleverDock.Models.ViewModels;

public class ThemeViewModel : ViewModelBase
{
    private readonly ThemeModel model = new();

    public ThemeViewModel(ThemeModel _model)
    {
        model = _model;
        LoadCommand = new ActionCommand(LoadAction);
    }

    /// <summary>
    ///     Name of the theme.
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

    /// <summary>
    ///     Path of the theme.
    /// </summary>
    public string Path
    {
        get => model.Path;
        set
        {
            if (value != model.Path)
            {
                model.Path = value;
                OnPropertyChanged();
            }
        }
    }

    public ActionCommand LoadCommand { get; private set; }

    private void LoadAction()
    {
        ThemeManager.Manager.LoadTheme(model);
    }
}