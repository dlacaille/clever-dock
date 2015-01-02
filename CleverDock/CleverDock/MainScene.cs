using CleverDock.Graphics;
using CleverDock.Graphics.Views;
using CleverDock.Managers;
using CleverDock.Tools;
using CleverDock.Views;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using D2D = SharpDX.Direct2D1;
using DWrite = SharpDX.DirectWrite;
using WIC = SharpDX.WIC;

namespace CleverDock
{
    class MainScene : Scene
    {
        private PrimitiveBatch<VertexPositionColor> primitiveBatch;
        private VertexPositionColor[] vertices;
        private BasicEffect basicEffect;

        public MainScene()
        {
            //View.Subviews.Add(fpsCounter = new FPSCounterView(new Rectangle(20, 60, 80, 20)));
            //View.Subviews.Add(dock = new Dock(this));
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            ToDisposeContent(primitiveBatch = new PrimitiveBatch<VertexPositionColor>(GraphicsDevice));
            ToDisposeContent(basicEffect = new BasicEffect(GraphicsDevice)
                {
                    VertexColorEnabled = true,
                    View = Matrix.Identity,
                    Projection = Matrix.Identity,
                    World = Matrix.Identity
                });
            vertices = new VertexPositionColor[] {
                                                     new VertexPositionColor(new Vector3(-1,-1,0), Color.Red),
                                                     new VertexPositionColor(new Vector3(-1,1,0), Color.Green),
                                                     new VertexPositionColor(new Vector3(1,1,0), Color.Blue),
                                                     new VertexPositionColor(new Vector3(1,-1,0), Color.Purple)
                                                 };
        }

        protected override void Draw(SharpDX.Toolkit.GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            basicEffect.CurrentTechnique.Passes[0].Apply();
            primitiveBatch.Begin();
            primitiveBatch.DrawQuad(vertices[0], vertices[1], vertices[2], vertices[3]);
            primitiveBatch.End();

            base.Draw(gameTime);
        }
    }
}
