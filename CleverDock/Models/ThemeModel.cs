using System.IO;

namespace CleverDock.Models;

public class ThemeModel
{
    public ThemeModel()
    {
    }

    public ThemeModel(string path)
    {
        var fileInfo = new FileInfo(path);
        var settings = new ThemeSettingsModel(fileInfo.Directory.FullName);
        Name = settings.ThemeName;
        Path = fileInfo.Directory.FullName;
    }

    /// <summary>
    ///     Name of the theme.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Path of the theme.
    /// </summary>
    public string Path { get; set; }
}