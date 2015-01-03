using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Graphics.Interfaces
{
    public interface IComponent
    {
        string Name { get; set; }

        void Initialize();
    }
}
