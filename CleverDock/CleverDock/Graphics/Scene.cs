using CleverDock.Graphics.Views;
using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Windows;
using System.Windows.Input;
using D2D = SharpDX.Direct2D1;
using D3D10 = SharpDX.Direct3D10;
using DXGI = SharpDX.DXGI;

namespace CleverDock.Graphics
{
    /// <summary>Represents a Direct2D drawing.</summary>
    public abstract class Scene : Game
    {
        #region Private Fields

        private GraphicsDeviceManager graphicsDeviceManager;
        private View capturedMouseView;

        #endregion
        #region Properties

        public View View { get; set; }
        public SharpDXElement WindowElement { get { return Window == null ? null : Window.NativeWindow as SharpDXElement; } }
        public View CapturedMouseView 
        { 
            get
            {
                return capturedMouseView;
            }
            set
            {
                Mouse.Capture(value == null ? null : WindowElement);
                capturedMouseView = value;
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
        /// Event triggered when the mouse is moved within the frame of the scene.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseMove;

        /// <summary>
        /// Event triggered when the mouse enters the frame of the scene.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseEnter;

        /// <summary>
        /// Event triggered when the mouse leaves the frame of the scene.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseLeave;

        /// <summary>
        /// Event triggered when the mouse stops being captured by the scene.
        /// </summary>
        public event EventHandler<MouseEventArgs> LostMouseCapture;

        #endregion

        #region Constructors and Deconstructors

        /// <summary>
        /// Initializes a new instance of the Scene class.
        /// </summary>
        protected Scene()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

            // Create a superview.
            GameSystems.Add(View = new View(this));

            // Assign the window events.
            WindowCreated += Scene_WindowCreated;
        }

        void Scene_WindowCreated(object sender, EventArgs e)
        {
            WindowElement.MouseLeftButtonDown += Window_MouseLeftButtonDown;
            WindowElement.MouseLeftButtonUp += Window_MouseLeftButtonUp;
            WindowElement.MouseMove += Window_MouseMove;
            WindowElement.MouseEnter += Window_MouseEnter;
            WindowElement.MouseLeave += Window_MouseLeave;
            WindowElement.LostMouseCapture += Window_LostMouseCapture;
        }

        #endregion
        #region Mouse Events
        
        void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonDown(this, e);
        }

        void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonUp(this, e);
        }

        void Window_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMove(this, e);
        }

        void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnter(this, e);
        }

        void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseLeave(this, e);
        }

        void Window_LostMouseCapture(object sender, MouseEventArgs e)
        {
            CapturedMouseView = null;
            LostMouseCapture(this, e);
        }

        #endregion
    }
}
