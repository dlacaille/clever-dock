using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace CleverDock.Tools;

public class XamlHelper
{
    public static ResourceDictionary LoadXaml(string path)
    {
        ResourceDictionary resourceDict;
        using (var fs = new FileStream(path, FileMode.Open))
        {
            resourceDict = (ResourceDictionary)XamlReader.Load(fs);
        }

        return resourceDict;
    }
}