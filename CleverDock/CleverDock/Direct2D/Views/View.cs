using CleverDock.Direct2D.Scenes;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleverDock.Direct2D.Views
{
    public class View : IDisposable
    {
        private Rectangle bounds;

        /// <summary>
        /// Allows the view to be rendered.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Bounds of the view, relative to its parent.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
                RecalculateFrame();
            }
        }

        /// <summary>
        /// Frame of the view, relative to the scene.
        /// </summary>
        public RectF Frame { get; set; }

        /// <summary>
        /// Subviews of the view which will be rendered over the view.
        /// </summary>
        public ViewCollection Subviews { get; private set; }

        /// <summary>
        /// View which contains this view.
        /// </summary>
        public View Superview { get; internal set; }

        /// <summary>
        /// Scene which contains the view.
        /// </summary>
        public Scene Scene { get; internal set; }

        /// <summary>
        /// Gets the <see cref="D2D.RenderTarget"/> used for drawing.
        /// </summary>
        protected RenderTarget RenderTarget
        {
            get
            {
                if (Scene == null)
                    return null;
                return Scene.RenderTarget;
            }
        }

        public View(Rectangle bounds)
        {
            Subviews = new ViewCollection(this);
            Visible = true;
            Bounds = bounds;
        }

        public View(Scene scene)
            : this(new Rectangle())
        {
            Scene = scene;
        }

        ~View()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(Subviews != null)
            {
                foreach (var subView in Subviews)
                    subView.Dispose(disposing);
            }
            Subviews = null;
            Superview = null;
            Scene = null;
        }

        public void RecalculateFrame()
        {
            if (Superview != null)
            {
                Frame = new RectF(
                    Bounds.Left + Superview.Frame.Left,
                    Bounds.Top + Superview.Frame.Top,
                    Bounds.Right + Superview.Frame.Left,
                    Bounds.Bottom + Superview.Frame.Top);
            }
            else
                Frame = new RectF(
                    Bounds.Left,
                    Bounds.Top,
                    Bounds.Right,
                    Bounds.Bottom);
            foreach (var view in Subviews)
                view.RecalculateFrame();
        }

        /// <summary>
        /// Removes this view from its superview.
        /// </summary>
        public void RemoveFromSuperview()
        {
            if (Superview == null)
                return;
            Superview.Subviews.Remove(this);
        }
        
        public void CreateResources()
        {
            OnCreateResources();
            foreach (var view in Subviews)
                view.CreateResources();
        }

        public void FreeResources()
        {
            OnFreeResources();
            foreach (var view in Subviews)
                view.FreeResources();
        }

        /// <summary>
        /// When overriden in a derived class, creates device dependent resources.
        /// </summary>
        protected virtual void OnCreateResources() { }

        /// <summary>
        /// When overriden in a deriven class, releases device dependent resources.
        /// </summary>
        protected virtual void OnFreeResources() { }

        /// <summary>
        /// When overriden in a derived class, renders the Direct2D content.
        /// </summary>
        protected virtual void OnRender() { }

        public void Render()
        {
            if (!Visible)
                return;
            OnRender();
            foreach(var subView in Subviews)
                subView.Render();
        }
    }
}
