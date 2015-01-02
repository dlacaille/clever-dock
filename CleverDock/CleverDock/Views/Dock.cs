using CleverDock.Graphics;
using CleverDock.Handlers;
using CleverDock.Model;
using CleverDock.Views.Decorators;
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
        public IEnumerable<DockIcon> Icons 
        { 
            get 
            {
                return Subviews.OfType<DockIcon>();
            }
        }
        public DockBackground Background { get; set; }

        public Dock(Scene scene)
            : base(scene)
        {
            this.Subviews.Add(Background = new DockBackground(scene));
            this.Subviews.Added += Subviews_Added;
            this.Subviews.Removed += Subviews_Removed;
            new DockWindowManagerDecorator(this);
        }

        private void LayoutIcons()
        {
            var icons = Icons.ToList();
            for(int i=0; i<icons.Count; i++)
            {
                var icon = icons[i];
                var iconSize = 48;
                icon.Bounds = new RectangleF(i * (iconSize + 8) + 4, 0, iconSize, iconSize);
            }
        }

        void Subviews_Added(object sender, ViewEventArgs e)
        {
            LayoutIcons();
        }

        void Subviews_Removed(object sender, ViewEventArgs e)
        {
            LayoutIcons();
        }

        public override RectangleF Frame
        {
            get
            {
                if (Scene == null)
                    return new RectangleF();
                var screenSize = Scene.View.Bounds.Size;
                var iconSize = 48 + 8;
                var dockSize = new Size2F(iconSize * Icons.ToList().Count, 70);
                return new RectangleF(
                    (float)Math.Round((screenSize.Width - dockSize.Width) / 2),
                    screenSize.Height - dockSize.Height,
                    dockSize.Width,
                    dockSize.Height);
            }
        }
    }
}
