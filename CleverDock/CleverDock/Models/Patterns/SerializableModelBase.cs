using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Patterns
{
    public class SerializableModelBase
    {
        public void LoadFromFile(string path)
        {
            if (!File.Exists(path))
                return;
            using (var stream = new StreamReader(path))
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

        public void SaveAsFile(string path)
        {
            var json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            using (var stream = new StreamWriter(path, false))
                stream.Write(json);
        }

        public bool CanDeserialize(string path)
        {
            if (!File.Exists(path))
                return false;
            return true; // TODO: Check if json is valid.
        }
    }
}
