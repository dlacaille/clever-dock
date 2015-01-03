using CleverDock.Graphics.Interfaces;
using CleverDock.Graphics.Views;
using CleverDock.Managers;
using SharpDX;
using System;
using System.Windows;
using System.Windows.Input;
using D2D = SharpDX.Direct2D1;
using D3D10 = SharpDX.Direct3D10;
using DXGI = SharpDX.DXGI;

namespace CleverDock.Graphics
{
    /// <summary>Represents a Direct2D drawing.</summary>
    public abstract class Scene : IDisposable, IDrawable, IContentable
    {
        #region Private Fields

        private readonly D2D.Factory factory;
        private readonly DisposableContentCollector collector;
        private D3D10.Device device;
        private D2D.RenderTarget renderTarget;
        private D3D10.Texture2D texture;
        private bool disposed;
        private object renderTargetLock = new object();
        private View capturedMouseView;

        #endregion
        #region Properties

        public View View { get; set; }
        public Window Window { get; set; }
        public View CapturedMouseView 
        { 
            get
            {
                return capturedMouseView;
            }
            set
            {
                Mouse.Capture(value == null ? null : Window);
                capturedMouseView = value;
            }
        }
        public bool IsVisible { get; set; }

        #endregion
        #region DirectX Resources

        /// <summary>Gets the surface this instance draws to.</summary>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose()"/> has been called on this instance.
        /// </exception>
        public D3D10.Texture2D Texture
        {
            get
            {
                this.ThrowIfDisposed();
                return this.texture;
            }
        }

        /// <summary>
        /// Gets the <see cref="D2D.D2DFactory"/> used to create the resources.
        /// </summary>
        protected D2D.Factory Factory
        {
            get { return this.factory; }
        }


        /// <summary>
        /// Gets the <see cref="D2D.RenderTarget"/> used for drawing.
        /// </summary>
        public D2D.RenderTarget RenderTarget
        {
            get { return this.renderTarget; }
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

        /// <summary>
        /// Raised when the content of the Scene has changed.
        /// </summary>
        public event EventHandler Updated;

        #endregion
        #region ToDispose

        protected void ToDispose(IDisposable disposableContent)
        {
            collector.Add(disposableContent);
        }

        #endregion

        #region Constructors and Deconstructors

        /// <summary>
        /// Initializes a new instance of the Scene class.
        /// </summary>
        protected Scene(Window window)
        {
            // We'll create a multi-threaded one to make sure it plays nicely with WPF
            factory = new D2D.Factory(D2D.FactoryType.MultiThreaded);
            collector = new DisposableContentCollector();
            View = new View(this);
            Window = window;
            Window.MouseLeftButtonDown += Window_MouseLeftButtonDown;
            Window.MouseLeftButtonUp += Window_MouseLeftButtonUp;
            Window.MouseMove += Window_MouseMove;
            Window.MouseEnter += Window_MouseEnter;
            Window.MouseLeave += Window_MouseLeave;
            Window.LostMouseCapture += Window_LostMouseCapture;
            IsVisible = true;
        }

        /// <summary>
        /// Finalizes an instance of the Scene class.
        /// </summary>
        ~Scene()
        {
            Dispose(false);
        }

        /// <summary>
        /// Immediately frees any system resources that the object might hold.
        /// </summary>
        public void Dispose()
        {
            if (View != null)
            {
                View.Dispose();
                View = null;
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Immediately frees any system resources that the object might hold.
        /// </summary>
        /// <param name="disposing">
        /// Set to true if called from an explicit disposer; otherwise, false.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            FreeDevice();
            if (disposing)
                factory.Dispose();
            disposed = true;
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
        #region Resizing

        /// <summary>Resizes the scene.</summary>
        /// <param name="width">The new width for the scene.</param>
        /// <param name="height">The new height for the scene.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// width/height is less than zero.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="LoadContent"/> has not been called.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose()"/> has been called on this instance.
        /// </exception>
        /// <exception cref="DirectX.DirectXException">
        /// An error occured creating device dependent resources.
        /// </exception>
        public void Resize(int width, int height)
        {
            this.ThrowIfDisposed();
            if (width < 0)
                throw new ArgumentOutOfRangeException("width", "Value must be positive.");
            if (height < 0)
                throw new ArgumentOutOfRangeException("height", "Value must be positive.");
            if (this.device == null)
                throw new InvalidOperationException("CreateResources has not been called.");

            lock (renderTargetLock)
            {
                // Recreate the render target
                this.CreateTexture(width, height);
                using (var surface = this.texture.QueryInterface<DXGI.Surface>())
                {
                    this.CreateRenderTarget(surface);
                }

                // Resize our viewport
                var viewport = new Viewport();
                viewport.Height = height;
                viewport.MaxDepth = 1;
                viewport.MinDepth = 0;
                viewport.X = 0;
                viewport.Y = 0;
                viewport.Width = width;
                this.device.Rasterizer.SetViewports(new Viewport[] { viewport });
            }

            // Resize main view.
            if (View != null)
                View.Bounds = new Rectangle(0, 0, width, height);

            // Destroy and recreate any dependent resources declared in a
            // derived class only (i.e don't destroy our resources).
            (this as IContentable).UnloadContent();
            (this as IContentable).LoadContent();
        }
        
        private void OnUpdated()
        {
            var callback = this.Updated;
            if (callback != null)
                callback(this, EventArgs.Empty);
        }

        #endregion
        #region Device Interface

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if
        /// <see cref="Dispose()"/> has been called on this instance.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        private static D3D10.Device1 TryCreateDevice1(D3D10.DriverType type)
        {
            // We'll try to create the device that supports any of these feature levels
            D3D10.FeatureLevel[] levels =
            {
                D3D10.FeatureLevel.Level_10_1,
                D3D10.FeatureLevel.Level_10_0,
                D3D10.FeatureLevel.Level_9_3,
                D3D10.FeatureLevel.Level_9_2,
                D3D10.FeatureLevel.Level_9_1
            };

            foreach (var level in levels)
            {
                try
                {
                    var device = new D3D10.Device1(type, D3D10.DeviceCreationFlags.BgraSupport, level);
                    Console.WriteLine("Created device with feature level '{0}'", level.ToString());
                    return device;
                }
                catch (ArgumentException) // E_INVALIDARG
                {
                    continue; // Try the next feature level
                }
                catch (OutOfMemoryException) // E_OUTOFMEMORY
                {
                    continue; // Try the next feature level
                }
                catch (SharpDXException) // D3DERR_INVALIDCALL or E_FAIL
                {
                    continue; // Try the next feature level
                }
            }
            return null; // We failed to create a device at any required feature level
        }

        private void CreateRenderTarget(DXGI.Surface surface)
        {
            // Create a D2D render target which can draw into our offscreen D3D
            // surface. D2D uses device independant units, like WPF, at 96/inch
            var properties = new D2D.RenderTargetProperties();
            properties.DpiX = 96;
            properties.DpiY = 96;
            properties.MinLevel = D2D.FeatureLevel.Level_DEFAULT;
            properties.PixelFormat = new D2D.PixelFormat(DXGI.Format.Unknown, D2D.AlphaMode.Premultiplied);
            properties.Type = D2D.RenderTargetType.Default;
            properties.Usage = D2D.RenderTargetUsage.None;

            // Assign result to temporary variable in case CreateGraphicsSurfaceRenderTarget throws
            var target = new D2D.RenderTarget(factory, surface, properties);

            lock (renderTargetLock)
            {
                if (this.renderTarget != null)
                    this.renderTarget.Dispose();
                this.renderTarget = target;
            }
        }

        private void CreateTexture(int width, int height)
        {
            var description = new D3D10.Texture2DDescription();
            description.ArraySize = 1;
            description.BindFlags = D3D10.BindFlags.RenderTarget | D3D10.BindFlags.ShaderResource;
            description.CpuAccessFlags = D3D10.CpuAccessFlags.None;
            description.Format = DXGI.Format.B8G8R8A8_UNorm;
            description.MipLevels = 1;
            description.OptionFlags = D3D10.ResourceOptionFlags.Shared;
            description.SampleDescription = new DXGI.SampleDescription(1, 0);
            description.Usage = D3D10.ResourceUsage.Default;

            description.Height = height;
            description.Width = width;

            // Assign result to temporary variable in case CreateTexture2D throws
            var texture = new D3D10.Texture2D(device, description);
            if (this.texture != null)
                this.texture.Dispose();
            this.texture = texture;
        }

        #endregion

        #region IDrawable Members

        void IDrawable.Draw()
        {
            if (!IsVisible)
                return;
            ThrowIfDisposed();
            if (this.renderTarget == null)
                throw new InvalidOperationException("Resize has not been called.");
            lock (renderTargetLock)
            {
                if (BeginDraw())
                {
                    renderTarget.BeginDraw();
                    Draw();
                    var drawableView = View as IDrawable;
                    if (drawableView.BeginDraw())
                    {
                        drawableView.Draw();
                        drawableView.EndDraw();
                    }
                    renderTarget.EndDraw();
                    EndDraw();
                }
            }
            device.Flush();
            OnUpdated();
        }

        public virtual bool BeginDraw() 
        {
            return true;
        }

        public virtual void Draw() { }

        public virtual void EndDraw() { }

        #endregion
        #region IContentable Members

        public void CreateDevice()
        {
            this.ThrowIfDisposed();
            if (this.device != null)
                throw new InvalidOperationException("A previous call to CreateDevice has not been followed by a call to FreeDevice.");

            // Try to create a hardware device first and fall back to a
            // software (WARP doens't let us share resources)
            var device1 = TryCreateDevice1(D3D10.DriverType.Hardware);
            if (device1 == null)
            {
                device1 = TryCreateDevice1(D3D10.DriverType.Software);
                if (device1 == null)
                    throw new SharpDXException("Unable to create a DirectX 10 device.");
            }
            this.device = device1.QueryInterface<D3D10.Device>();
            device1.Dispose();
        }

        public void FreeDevice()
        {
            UnloadContent();
            if (this.texture != null)
            {
                this.texture.Dispose();
                this.texture = null;
            }
            lock (renderTargetLock)
            {
                if (this.renderTarget != null)
                {
                    this.renderTarget.Dispose();
                    this.renderTarget = null;
                }
            }
            if (this.device != null)
            {
                this.device.Dispose();
                this.device = null;
            }
        }

        void IContentable.LoadContent()
        {
            LoadContent();
            if (View != null)
                (View as IContentable).LoadContent();
        }

        protected virtual void LoadContent() { }

        /// <summary>
        /// Releases the DirectX device and any device dependent resources.
        /// </summary>
        /// <remarks>
        /// This method is safe to be called even if the instance has been disposed.
        /// </remarks>
        void IContentable.UnloadContent()
        {
            collector.DisposeContent();
            UnloadContent();
            if (View != null)
                (View as IContentable).UnloadContent();
        }

        /// <summary>
        /// When overriden in a deriven class, releases device dependent resources.
        /// </summary>
        protected virtual void UnloadContent() { }

        #endregion
    }
}
