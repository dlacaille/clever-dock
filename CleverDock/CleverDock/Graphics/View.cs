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
using System.Windows.Input;
using SharpDX.Toolkit;
using System.ComponentModel;
using System.Windows;

namespace CleverDock.Graphics
{
    public class View : GameSystem
    {
        #region Private Fields

        private bool isMouseOver;
        private int drawOrder;
        private int updateOrder;
        private bool visible = true;
        private bool enabled = true;
        private bool contentLoaded;
        private readonly DisposeCollector contentCollector;
        private readonly ViewCollection subviews;

        private readonly List<IDrawable> currentlyDrawingGameSystems;
        private readonly List<IUpdateable> currentlyUpdatingGameSystems;
        private readonly List<IContentable> currentlyContentGameSystems;
        private readonly List<IDrawable> drawableGameSystems;
        private readonly List<IGameSystem> pendingGameSystems;
        private readonly List<IUpdateable> updateableGameSystems;
        private readonly List<IContentable> contentableGameSystems;

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
        public ViewCollection Subviews 
        {
            get { return subviews;  }
        }

        /// <summary>
        /// View which contains this view.
        /// </summary>
        public View Superview { get; internal set; }

        /// <summary>
        /// Scene which contains the view.
        /// </summary>
        public Scene Scene { get { return Game as Scene; } }

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

        public View(Game game)
            : base(game)
        {
            // Internals
            drawableGameSystems = new List<IDrawable>();
            currentlyContentGameSystems = new List<IContentable>();
            currentlyDrawingGameSystems = new List<IDrawable>();
            pendingGameSystems = new List<IGameSystem>();
            updateableGameSystems = new List<IUpdateable>();
            currentlyUpdatingGameSystems = new List<IUpdateable>();
            contentableGameSystems = new List<IContentable>();
            contentCollector = new DisposeCollector();

            // Subviews
            subviews = new ViewCollection(this);
            subviews.Added += Subviews_Added;
            subviews.Removed += Subviews_Removed;

            // Mouse events
            MouseLeftButtonDown += View_MouseLeftButtonDown;
            MouseLeftButtonUp += View_MouseLeftButtonUp;
            MouseMove += View_MouseMove;
            MouseLeave += View_MouseLeave;
            LostMouseCapture += View_LostMouseCapture;
        }

        public View(Scene scene)
            : this(scene as Game)
        {
            Scene.MouseLeftButtonDown += Scene_MouseLeftButtonDown;
            Scene.MouseLeftButtonUp += Scene_MouseLeftButtonUp;
            Scene.MouseMove += Scene_MouseMove;
            Scene.MouseEnter += Scene_MouseEnter;
            Scene.MouseLeave += Scene_MouseLeave;
            Scene.LostMouseCapture += Scene_LostMouseCapture;
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
            var pos = e.MouseDevice.GetPosition(Scene.WindowElement);
            var point = new SharpDX.Point((int)pos.X, (int)pos.Y);
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
            lock (Subviews)
            {
                foreach(var view in Subviews)
                {
                    view.MouseMove(view, e);
                    if (view.IsMouseOver && view != found)
                    {
                        view.isMouseOver = false;
                        if (view.MouseLeave != null)
                            view.MouseLeave(view, e);
                    }
                }
            }
        }

        void View_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Scene == null)
                return;
            var pos = e.MouseDevice.GetPosition(Scene.WindowElement);
            var point = new SharpDX.Point((int)pos.X, (int)pos.Y);
            var found = Subviews.LastOrDefault(s => s.Frame.Contains(point));
            if (found != null && found.MouseLeftButtonUp != null)
                found.MouseLeftButtonUp(found, e);
        }

        void View_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Scene == null)
                return;
            var pos = e.MouseDevice.GetPosition(Scene.WindowElement);
            var point = new SharpDX.Point((int)pos.X, (int)pos.Y);
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
        #region Comparers

        internal struct UpdateableSearcher : IComparer<IUpdateable>
        {
            public static readonly UpdateableSearcher Default = new UpdateableSearcher();

            public int Compare(IUpdateable left, IUpdateable right)
            {
                if (Equals(left, right))
                    return 0;
                if (left == null)
                    return 1;
                if (right == null)
                    return -1;
                return (left.UpdateOrder < right.UpdateOrder) ? -1 : 1;
            }
        }

        internal struct DrawableSearcher : IComparer<IDrawable>
        {
            public static readonly DrawableSearcher Default = new DrawableSearcher();

            public int Compare(IDrawable left, IDrawable right)
            {
                if (Equals(left, right))
                    return 0;
                if (left == null)
                    return 1;
                if (right == null)
                    return -1;
                return (left.DrawOrder < right.DrawOrder) ? -1 : 1;
            }
        }

        private static int UpdateableComparison(IUpdateable left, IUpdateable right)
        {
            return left.UpdateOrder.CompareTo(right.UpdateOrder);
        }

        private static int DrawableComparison(IDrawable left, IDrawable right)
        {
            return left.DrawOrder.CompareTo(right.DrawOrder);
        }

        #endregion
        #region Subview Management

        void Subviews_Added(object sender, Handlers.ViewEventArgs e)
        {
            var view = e.View;
            // Initialize the view if the game is running, else add it to the pending list.
            if (Game.IsRunning)
                view.Initialize();
            else
                pendingGameSystems.Add(view);
            // Add the view to the contentable systems.
            lock (contentableGameSystems)
            {
                if (!contentableGameSystems.Contains(view))
                    contentableGameSystems.Add(view);
            }
            // Load content if the game is running.
            if (Game.IsRunning)
                view.LoadContent();
            // Add the view to the updateable systems.
            if (AddGameSystem(view, updateableGameSystems, UpdateableSearcher.Default, UpdateableComparison))
                view.UpdateOrderChanged += view_UpdateOrderChanged;
            // Add the view to the drawable systems.
            if (AddGameSystem(view, drawableGameSystems, DrawableSearcher.Default, DrawableComparison))
                view.DrawOrderChanged += view_DrawOrderChanged;
            // Set view superview.
            view.Superview = this;
        }

        void Subviews_Removed(object sender, Handlers.ViewEventArgs e)
        {
            var view = e.View;
            // Remove from pending if the game is still not running.
            if (!Game.IsRunning)
                pendingGameSystems.Remove(view);
            // Remove from contentable systems.
            lock (contentableGameSystems)
            {
                contentableGameSystems.Remove(view);
            }
            // Unload content.
            view.UnloadContent();
            // Remove from updatable systems.
            lock (updateableGameSystems)
            {
                updateableGameSystems.Remove(view);
            }
            view.UpdateOrderChanged -= view_UpdateOrderChanged;
            // Remove from drawable systems.
            lock (drawableGameSystems)
            {
                drawableGameSystems.Remove(view);
            }
            view.DrawOrderChanged -= view_DrawOrderChanged;
            // Remove superview.
            e.View.Superview = null;
        }

        void view_UpdateOrderChanged(object sender, EventArgs e)
        {
            AddGameSystem((IUpdateable)sender, updateableGameSystems, UpdateableSearcher.Default, UpdateableComparison, true);
        }

        void view_DrawOrderChanged(object sender, EventArgs e)
        {
            AddGameSystem((IDrawable)sender, drawableGameSystems, DrawableSearcher.Default, DrawableComparison, true);
        }

        private static bool AddGameSystem<T>(T gameSystem, List<T> gameSystems, IComparer<T> comparer, Comparison<T> orderComparer, bool removePreviousSystem = false)
        {
            lock (gameSystems)
            {
                // If we are updating the order
                if (removePreviousSystem)
                    gameSystems.Remove(gameSystem);

                // Find this gameSystem
                int index = gameSystems.BinarySearch(gameSystem, comparer);
                if (index < 0)
                {
                    // If index is negative, that is the bitwise complement of the index of the next element that is larger than item 
                    // or, if there is no larger element, the bitwise complement of Count.
                    index = ~index;

                    // Iterate until the order is different or we are at the end of the list
                    while ((index < gameSystems.Count) && (orderComparer(gameSystems[index], gameSystem) == 0))
                        index++;

                    gameSystems.Insert(index, gameSystem);

                    // True, the system was inserted
                    return true;
                }
            }

            // False, it is already in the list
            return false;
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
        #region GameSystem members

        public override void Initialize()
        {
            base.Initialize();
            // Initialize all pending systems.
            while (pendingGameSystems.Count != 0)
            {
                pendingGameSystems[0].Initialize();
                pendingGameSystems.RemoveAt(0);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // Call virtual update method.
            Update(gameTime);
            // Update all updatable systems.
            lock (updateableGameSystems)
            {
                foreach (var updateable in updateableGameSystems)
                    currentlyUpdatingGameSystems.Add(updateable);
            }
            foreach (var updateable in currentlyUpdatingGameSystems)
            {
                if (updateable.Enabled)
                    updateable.Update(gameTime);
            }
            currentlyUpdatingGameSystems.Clear();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            // Load content for all contentable systems.
            lock (contentableGameSystems)
            {
                foreach (var contentable in contentableGameSystems)
                    currentlyContentGameSystems.Add(contentable);
            }
            foreach (var contentable in currentlyContentGameSystems)
                contentable.LoadContent();
            currentlyContentGameSystems.Clear();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            // Dispose all allocated objects in collector.
            contentCollector.DisposeAndClear();
            // Unload content for all contentable systems.
            lock (contentableGameSystems)
            {
                foreach (var contentable in contentableGameSystems)
                    currentlyContentGameSystems.Add(contentable);
            }
            foreach (var contentable in currentlyContentGameSystems)
                contentable.UnloadContent();
            currentlyContentGameSystems.Clear();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);     
            // Draw all drawable systems.
            lock (drawableGameSystems)
            {
                for (int i = 0; i < drawableGameSystems.Count; i++)
                    currentlyDrawingGameSystems.Add(drawableGameSystems[i]);
            }
            for (int i = 0; i < currentlyDrawingGameSystems.Count; i++)
            {
                var drawable = currentlyDrawingGameSystems[i];
                if (drawable.Visible)
                {
                    if (drawable.BeginDraw())
                    {
                        drawable.Draw(gameTime);
                        drawable.EndDraw();
                    }
                }
            }
            currentlyDrawingGameSystems.Clear();
        }

        #endregion
    }
}
