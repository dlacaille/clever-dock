using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Managers
{
    public class DisposableContentCollector
    {
        private List<IDisposable> DisposableContent { get; set; }

        public DisposableContentCollector()
        {
            DisposableContent = new List<IDisposable>();
        }

        public void DisposeContent()
        {
            lock (DisposableContent)
            {
                foreach (var content in DisposableContent)
                    content.Dispose();
                DisposableContent.Clear();
            }
        }

        public void Add(IDisposable disposableContent)
        {
            DisposableContent.Add(disposableContent);
        }

        public void Remove(IDisposable disposableContent)
        {
            DisposableContent.Remove(disposableContent);
        }
    }
}
