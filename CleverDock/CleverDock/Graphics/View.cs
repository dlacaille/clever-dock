﻿using CleverDock.Graphics;
using SharpDX;
using D2D = SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SharpDX.Direct2D1;
using System.Windows.Input;
using CleverDock.Graphics.Interfaces;
using IComponent = CleverDock.Graphics.Interfaces.IComponent;
using CleverDock.Managers;

namespace CleverDock.Graphics
{
    public class View : IDisposable, IDrawable, IContentable, IComponent
    {
        #region Private Fields

        private bool isMouseOver;
        private bool isLoaded;
        private readonly DisposableContentCollector collector;
        private readonly ViewCollection subviews;
        private SolidColorBrush redbrush;

        #endregion
        #region Properties

        /// <summary>
        /// Is true when the mouse is in the frame of the view.
        /// </summary>
        public bool IsMouseOver { get { return isMouseOver; } }

        /// <summary>
        /// Is treue when the mouse is captured by the view.
        /// </summary>
        public bool IsMouseCaptured { get { return Scene.CapturedMouseView == this; } }

        /// <summary>
        /// Allows the view to be rendered.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Name of the view.
        /// </summary>
        public virtual string Name { get; set; }

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
        public ViewCollection Subviews { get { return subviews; } }

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

        #endregion
        #region Events

        /// <summary>
        /// Event triggered when the left mouse button is released.
        /// </summary>
        public event EventHandler<MouseButtonEventArgs> MouseLeftButtonUp;

        /// <summary>
        /// Event triggered when the left mouse button is pressed.
        /// </summary>
        public event EventHandler<MouseButtonEventArgs> MouseLeftButtonDown;

        /// <summary>
        /// Event triggered when the mouse is moved within the frame of the view.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseMove;

        /// <summary>
        /// Event triggered when the mouse enters the frame of the view.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseEnter;

        /// <summary>
        /// Event triggered when the mouse leaves the frame of the view.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseLeave;

        /// <summary>
        /// Event triggered when the mouse stops being captured by the view.
        /// </summary>
        public event EventHandler<MouseEventArgs> LostMouseCapture;

        #endregion

        #region Constructors and Deconstructors

        public View()
        {
            IsVisible = true;
            collector = new DisposableContentCollector();
            subviews = new ViewCollection(this);
            Subviews.Added += Subviews_Added;
            Subviews.Removed += Subviews_Removed;
            MouseLeftButtonDown += View_MouseLeftButtonDown;
            MouseLeftButtonUp += View_MouseLeftButtonUp;
            MouseMove += View_MouseMove;
            MouseLeave += View_MouseLeave;
            LostMouseCapture += View_LostMouseCapture;
        }

        public View(Scene scene)
            : this()
        {
            Scene = scene;
            Scene.MouseLeftButtonDown += Scene_MouseLeftButtonDown;
            Scene.MouseLeftButtonUp += Scene_MouseLeftButtonUp;
            Scene.MouseMove += Scene_MouseMove;
            Scene.MouseEnter += Scene_MouseEnter;
            Scene.MouseLeave += Scene_MouseLeave;
            Scene.LostMouseCapture += Scene_LostMouseCapture;
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
            lock (subviews)
            {
                if (subviews != null)
                {
                    foreach (var subView in Subviews)
                        subView.Dispose(disposing);
                }
            }
            Subviews.Clear();
            Superview = null;
            Scene = null;
        }

        #endregion
        #region Scene Mouse Events

        void Scene_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseLeftButtonDown != null)
                MouseLeftButtonDown(this, e);
        }

        void Scene_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MouseLeftButtonUp != null)
                MouseLeftButtonUp(this, e);
        }

        void Scene_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseMove != null)
                MouseMove(this, e);
        }

        void Scene_MouseEnter(object sender, MouseEventArgs e)
        {
            if (MouseEnter != null)
                MouseEnter(this, e);
        }

        void Scene_MouseLeave(object sender, MouseEventArgs e)
        {
            if (MouseLeave != null)
                MouseLeave(this, e);
        }

        void Scene_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (LostMouseCapture != null)
                LostMouseCapture(this, e);
        }

        #endregion
        #region Mouse Events

        /// <summary>
        /// Captures the mouse so that mouse movement is always reported to the view.
        /// </summary>
        public void CaptureMouse()
        {
            Scene.CapturedMouseView = this;
        }

        void View_MouseMove(object sender, MouseEventArgs e)
        {
            if (Scene == null)
                return;
            // If a view requires capturing the mouse, send the event and break the bubbling.
            if (Scene.CapturedMouseView != null && Scene.CapturedMouseView != this)
            {
                Scene.CapturedMouseView.MouseMove(Scene.CapturedMouseView, e);
                return;
            }
            // Find the view which has a mouse over it.
            var pos = e.MouseDevice.GetPosition(Scene.Window);
            var point = new Point((int)pos.X, (int)pos.Y);
            var found = Subviews.LastOrDefault(s => s.Frame.Contains(point));
            if (found != null)
            {
                found.MouseMove(found, e);
                if (!found.isMouseOver)
                {
                    found.isMouseOver = true;
                    if (found.MouseEnter != null)
                        found.MouseEnter(found, e);
                }
            }
            // Set mouseOver to false for other views.
            for (int i = 0; i < Subviews.Count; i++)
            {
                Subviews[i].MouseMove(Subviews[i], e);
                if (Subviews[i].IsMouseOver && Subviews[i] != found)
                {
                    Subviews[i].isMouseOver = false;
                    if (Subviews[i].MouseLeave != null)
                        Subviews[i].MouseLeave(Subviews[i], e);
                }
            }
        }

        void View_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Scene == null)
                return;
            var pos = e.MouseDevice.GetPosition(Scene.Window);
            var point = new Point((int)pos.X, (int)pos.Y);
            var found = Subviews.LastOrDefault(s => s.Frame.Contains(point));
            if (found != null && found.MouseLeftButtonUp != null)
                found.MouseLeftButtonUp(found, e);
        }

        void View_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Scene == null)
                return;
            var pos = e.MouseDevice.GetPosition(Scene.Window);
            var point = new Point((int)pos.X, (int)pos.Y);
            var found = Subviews.LastOrDefault(s => s.Frame.Contains(point));
            if (found != null && found.MouseLeftButtonDown != null)
                found.MouseLeftButtonDown(found, e);
        }

        void View_MouseLeave(object sender, MouseEventArgs e)
        {
            // Set mouseOver to false for other views.
            for (int i = 0; i < Subviews.Count; i++)
            {
                Subviews[i].MouseMove(Subviews[i], e);
                if (Subviews[i].IsMouseOver)
                {
                    Subviews[i].isMouseOver = false;
                    if (Subviews[i].MouseLeave != null)
                        Subviews[i].MouseLeave(Subviews[i], e);
                }
            }
        }

        void View_LostMouseCapture(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < Subviews.Count; i++)
                if (Subviews[i].LostMouseCapture != null)
                    Subviews[i].LostMouseCapture(Subviews[i], e);
        }

        #endregion
        #region ToDispose

        protected void ToDispose(IDisposable disposableContent)
        {
            collector.Add(disposableContent);
        }

        #endregion
        #region Subview Management

        void Subviews_Added(object sender, Handlers.ViewEventArgs e)
        {
            if (RenderTarget == null)
                return;
            e.View.Scene = Scene;
            (e.View as IComponent).Initialize();
            (e.View as IContentable).LoadContent();
        }

        void Subviews_Removed(object sender, Handlers.ViewEventArgs e)
        {
            if (RenderTarget == null)
                return;
            e.View.Scene = null;
            (e.View as IContentable).UnloadContent();
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

        #endregion

        #region IComponent Members

        public virtual void Initialize() { }

        #endregion
        #region IContentable Members

        void IContentable.LoadContent()
        {
            if (isLoaded)
                return;
            isLoaded = true;
            ToDispose(redbrush = new SolidColorBrush(RenderTarget, new Color4(1, 0, 0, 1))); 
            LoadContent();
            lock (subviews)
            {
                foreach (IContentable view in Subviews)
                    view.LoadContent();
            }
        }

        /// <summary>
        /// When overriden in a derived class, creates device dependent resources.
        /// </summary>
        protected virtual void LoadContent() { }

        void IContentable.UnloadContent()
        {
            if (!isLoaded)
                return;
            isLoaded = false;
            collector.DisposeContent();
            UnloadContent();
            lock (Subviews)
            {
                foreach (IContentable view in Subviews)
                    view.UnloadContent();
            }
        }

        /// <summary>
        /// When overriden in a deriven class, releases device dependent resources.
        /// </summary>
        protected virtual void UnloadContent() { }

        #endregion
        #region IDrawable Members

        public virtual bool BeginDraw() 
        { 
            return true; 
        }

        public virtual void EndDraw() { }

        /// <summary>
        /// Calls OnRender and renders all subviews. This method will do nothing if Visible is set to false.
        /// </summary>
        void IDrawable.Draw()
        {
            if (!IsVisible)
                return;
            if (BeginDraw())
            {
                Draw();
                EndDraw();
            }
            lock (subviews)
            {
                foreach (IDrawable view in Subviews)
                    view.Draw();
            }
            RenderTarget.DrawRectangle(new RectangleF(Frame.X, Frame.Y, Frame.Width, Frame.Height), redbrush);
        }

        public virtual void Draw() { }

        #endregion
    }
}