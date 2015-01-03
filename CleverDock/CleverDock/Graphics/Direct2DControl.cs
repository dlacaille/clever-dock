using CleverDock.Graphics;
using CleverDock.Graphics.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CleverDock.Graphics
{
    /// <summary>Hosts a <see cref="Scene"/> instance.</summary>
    public sealed class Direct2DControl : Control
    {
        /// <summary>
        /// Identifies the <see cref="Scene"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SceneProperty = DependencyProperty.Register("Scene", typeof(Scene), typeof(Direct2DControl), new UIPropertyMetadata(SceneChangedCallback));

        private Image image;
        private D3D10Image imageSource;

        /// <summary>
        /// Initializes a new instance of the Direct2DControl class.
        /// </summary>
        public Direct2DControl()
        {
            imageSource = new D3D10Image();
            imageSource.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            image = new Image();
            image.Stretch = Stretch.Fill; // We set this because our resizing isn't immediate
            image.Source = imageSource;
            AddVisualChild(image);

            // To greatly reduce flickering we're only going to AddDirtyRect
            // when WPF is rendering.
            CompositionTarget.Rendering += OnRendering;
        }

        /// <summary>
        /// Gets or sets the <see cref="Direct2D.Scene"/> object displayed
        /// by this control.
        /// </summary>
        /// <remarks>
        /// The caller is resposible for the lifetime management of the Scene.
        /// </remarks>
        public Scene Scene
        {
            get { return (Scene)GetValue(SceneProperty); }
            set { SetValue(SceneProperty, value); }
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <remarks>
        /// This will always return 1 as this control hosts a single
        /// Image control.
        /// </remarks>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <summary>Arranges and sizes the child Image control.</summary>
        /// <param name="finalSize">The size used to arrange the control.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            image.Arrange(new Rect(0, 0, size.Width, size.Height));
            return size;
        }

        /// <summary>Returns the child Image control.</summary>
        /// <param name="index">
        /// The zero-based index of the requested child element in the collection.
        /// </param>
        /// <returns>The child Image control.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero or greater than VisualChildrenCount.
        /// </exception>
        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException("index");
            return image;
        }

        /// <summary>
        /// Updates the UIElement.DesiredSize of the child Image control.
        /// </summary>
        /// <param name="availableSize">The size that the control should not exceed.</param>
        /// <returns>The child Image's desired size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            image.Measure(availableSize);
            return image.DesiredSize;
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="sizeInfo">
        /// The packaged parameters, which includes old and new sizes, and which
        /// dimension actually changes.
        /// </param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (Scene != null)
            {
                // Check we don't resize too small
                int width = Math.Max(1, (int)ActualWidth);
                int height = Math.Max(1, (int)ActualHeight);
                Scene.Resize(width, height);

                imageSource.Lock();
                imageSource.SetBackBuffer(Scene.Texture);
                imageSource.Unlock();

                (Scene as IDrawable).Draw();
            }
        }

        private static void SceneChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (Direct2DControl)d;
            instance.OnSceneChanged();
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (this.imageSource.IsFrontBufferAvailable)
            {
                imageSource.Lock();
                (Scene as IDrawable).Draw();
                imageSource.AddDirtyRect(new Int32Rect(0, 0, imageSource.PixelWidth, imageSource.PixelHeight));
                imageSource.Unlock();
            }
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Scene != null)
            {
                if (imageSource.IsFrontBufferAvailable)
                    OnSceneChanged(); // Recreate the resources
                else
                    Scene.FreeDevice();
            }
        }

        private void OnSceneChanged()
        {
            imageSource.Lock();
            try
            {
                if (Scene != null)
                {
                    Scene.CreateDevice();

                    // Resize to the size of this control, if we have a size
                    int width = Math.Max(1, (int)ActualWidth);
                    int height = Math.Max(1, (int)ActualHeight);
                    Scene.Resize(width, height);

                    imageSource.SetBackBuffer(Scene.Texture);
                    (Scene as IDrawable).Draw();
                }
                else
                {
                    imageSource.SetBackBuffer(null);
                }
            }
            finally
            {
                imageSource.Unlock();
            }
        }
    }
}
