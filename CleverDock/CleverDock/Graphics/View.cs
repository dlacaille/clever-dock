using CleverDock.Graphics;
using SharpDX;
using D2D = SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SharpDX.Direct2D1;

namespace CleverDock.Graphics
{
    public class View : IDisposable
    {
        private bool isMouseOver;
        public bool IsMouseOver { get { return isMouseOver; } }

        /// <summary>
        /// Allows the view to be rendered.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Bounds of the view, relative to its parent.
        /// </summary>
        public RectangleF Bounds { get; set; }

        /// <summary>
        /// Frame of the view, relative to the scene.
        /// </summary>
        public virtual RectangleF Frame
        {
            get
            {
                if (Superview != null)
                    return new RectangleF(Superview.Frame.X + Bounds.X, Superview.Frame.Y + Bounds.Y, Bounds.Width, Bounds.Height);
                return Bounds;
            }
        }

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
        protected D2D.RenderTarget RenderTarget
        {
            get
            {
                if (Superview != null)
                    return Superview.RenderTarget;
                if (Scene != null)
                    return Scene.RenderTarget;
                return null;
            }
        }

        public View()
        {
            Subviews = new ViewCollection(this);
            Subviews.Added += Subviews_Added;
            Subviews.Removed += Subviews_Removed;
            Visible = true;
        }

        void Subviews_Added(object sender, Handlers.ViewEventArgs e)
        {
            if (RenderTarget == null)
                return;
            e.View.FreeResources();
            e.View.CreateResources();
        }

        void Subviews_Removed(object sender, Handlers.ViewEventArgs e)
        {
            if (RenderTarget == null)
                return;
            e.View.FreeResources();
        }

        public View(Scene scene)
            : this()
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

        public void Click(Point mousePos)
        {
            var found = Subviews.LastOrDefault(s => s.Frame.Contains(mousePos));
            if (found != null)
                found.Click(mousePos);
            OnClick(mousePos);
        }

        protected virtual void OnClick(Point mousePos) { }

        public void MouseMove(Point mousePos)
        {
            // Find the view which has a mouse over it.
            var found = Subviews.LastOrDefault(s => s.Frame.Contains(mousePos));
            if (found != null)
            {
                found.MouseMove(mousePos);
                found.isMouseOver = true;
            }

            // Set mouseOver to false for other views.
            for (int i = 0; i < Subviews.Count; i++)
            {
                Subviews[i].MouseMove(mousePos);
                if (Subviews[i].IsMouseOver && Subviews[i] != found)
                    Subviews[i].isMouseOver = false;
            }

            OnMouseMove(mousePos);
        }

        protected virtual void OnMouseMove(Point mousePos) { }

        public void MouseLeave()
        {
            for (int i = 0; i < Subviews.Count; i++)
            {
                Subviews[i].MouseLeave();
                if (Subviews[i].IsMouseOver)
                    Subviews[i].isMouseOver = false;
            }

            OnMouseLeave();
        }

        protected virtual void OnMouseLeave() { }

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
            for (int i = 0; i < Subviews.Count(); i++)
                Subviews[i].CreateResources();
        }

        public void FreeResources()
        {
            OnFreeResources();
            for (int i = 0; i < Subviews.Count(); i++)
                Subviews[i].FreeResources();
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
            for (int i = 0; i < Subviews.Count(); i++)
                Subviews[i].Render();
        }
    }
}
