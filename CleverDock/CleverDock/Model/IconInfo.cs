using System.Collections.Generic;
using System.IO;

namespace CleverDock.Model
{
    /// <summary>
    ///   Stores the information for a dock icon.
    /// </summary>
    public class IconInfo : SerializableIconInfo
    {
        public IconInfo()
        {
        }

        public IconInfo(SerializableIconInfo info)
        {
            Name = info.Name;
            Path = info.Path;
            ImagePath = info.ImagePath;
        }

        /// <summary>
        /// When the icon is pinned, it stays in the dock
        /// </summary>
        public bool Pinned { get; set; }
    }
}