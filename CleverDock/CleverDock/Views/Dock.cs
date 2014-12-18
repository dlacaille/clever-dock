using CleverDock.Graphics;
using CleverDock.Model;
using SharpDX;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Views
{
    public class Dock : View
    {
        public List<DockIcon> Icons { get; set; }
        public DockBackground Background { get; set; }

        public Dock()
            : base(null)
        {
            this.Subviews.Add(Background = new DockBackground());
        }

        public override RectangleF Frame
        {
            get
            {
                if (Scene == null)
                    return new RectangleF();
                var screenSize = Scene.View.Bounds.Size;
                var dockSize = new Size2F(200, 64);
                return new RectangleF(
                    (float)Math.Round((screenSize.Width - dockSize.Width) / 2),
                    screenSize.Height - dockSize.Height,
                    dockSize.Width,
                    dockSize.Height);
            }
        }
    }
}
