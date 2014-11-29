using System;
using System.IO;

namespace CleverDock.Tools
{
    public class PathTools
    {
        public static bool SamePath(string a, string b)
        {
            if (a == null || b == null)
                return a == b;
            return String.Compare(
                    Path.GetFullPath(a).TrimEnd('\\'),
                    Path.GetFullPath(b).TrimEnd('\\'),
                    StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
