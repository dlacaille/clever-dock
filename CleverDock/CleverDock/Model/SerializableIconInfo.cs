using System.IO;

namespace CleverDock.Model
{
    /// <summary>
    ///   Stores the information for a dock icon.
    /// </summary>
    public class SerializableIconInfo
    {
        public SerializableIconInfo()
        {
        }

        public SerializableIconInfo(SerializableIconInfo info)
        {
            Path = info.Path;
            ImagePath = info.ImagePath;
            Name = info.Name;
        }

        public SerializableIconInfo(string name, string path)
        {
            Name = name;
            Path = path;
        }

        /// <summary>
        ///   The path to the icon's executable or file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///   Optional. The path to the image used for the icon, if set to null the system icon will be used.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        ///   The name of the icon
        /// </summary>
        public string Name { get; set; }
    }
}