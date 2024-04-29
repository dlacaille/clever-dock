using CleverDock.Models;

namespace CleverDock.Handlers;

public class ThemeEventArgs : EventArgs
{
    public ThemeEventArgs(ThemeModel theme)
    {
        Theme = theme;
    }

    public ThemeModel Theme { get; set; }
}