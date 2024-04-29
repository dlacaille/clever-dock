using System.IO;
using CleverDock.Patterns;
using Newtonsoft.Json;

namespace CleverDock.Models;

public class ThemeSettingsModel : SerializableModelBase
{
    public ThemeSettingsModel()
    {
        IconPaddingX = 15;
        IconPaddingY = 20;
    }

    public ThemeSettingsModel(string path)
    {
        var filename = Path.Combine(path, "theme.json");
        if (!File.Exists(filename))
            return;
        using (var stream = new StreamReader(filename))
        {
            try
            {
                JsonConvert.PopulateObject(stream.ReadToEnd(), this);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    /// <summary>
    ///     Name of the Theme.
    /// </summary>
    public string ThemeName { get; set; }

    /// <summary>
    ///     Author of the Theme.
    /// </summary>
    public string Author { get; set; }

    /// <summary>
    ///     Version of the Theme.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    ///     Description of the Theme.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     X icon padding.
    /// </summary>
    public int IconPaddingX { get; set; }

    /// <summary>
    ///     Y icon padding
    /// </summary>
    public int IconPaddingY { get; set; }
}