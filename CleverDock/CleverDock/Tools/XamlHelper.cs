using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace CleverDock.Tools
{
    public class XamlHelper
    {
        public static ResourceDictionary LoadXaml(string path)
        {
            ResourceDictionary resourceDict;
            using (FileStream fs = new FileStream(path, FileMode.Open))
                resourceDict = (ResourceDictionary)XamlReader.Load(fs);
            return resourceDict;
        }
    }
}
