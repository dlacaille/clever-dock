using System;
using System.IO;

namespace CleverDock.Tools
{
    public class PathTools
    {
        public static bool SamePath(string a, string b)
        {
            if (String.IsNullOrEmpty(a) || String.IsNullOrEmpty(b))
                return a == b;
            return String.Compare(
                    Path.GetFullPath(a).TrimEnd('\\'),
                    Path.GetFullPath(b).TrimEnd('\\'),
                    StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
