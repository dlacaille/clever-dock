using CleverDock.Patterns;
using System.IO;

namespace CleverDock.Models
{
    /// <summary>
    ///   Stores the information for a dock icon.
    /// </summary>
    public class IconModel : SerializableModelBase
    {
        /// <summary>
        /// The path to the icon's executable or file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Optional. The path to the image used for the icon, if set to null the system icon will be used.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// The name of the icon
        /// </summary>
        public string Name { get; set; }
    }
}