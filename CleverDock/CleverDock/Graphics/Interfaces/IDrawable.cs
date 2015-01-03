using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Graphics.Interfaces
{
    public interface IDrawable
    {
        bool IsVisible { get; }

        bool BeginDraw();
        void Draw();
        void EndDraw();
    }
}
