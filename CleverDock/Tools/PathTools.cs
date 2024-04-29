using System.IO;

namespace CleverDock.Tools;

public class PathTools
{
    public static bool SamePath(string a, string b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            return a == b;
        return string.Compare(
            Path.GetFullPath(a).TrimEnd('\\'),
            Path.GetFullPath(b).TrimEnd('\\'),
            StringComparison.InvariantCultureIgnoreCase) == 0;
    }
}