using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine
{
    internal class Renderer
    {
        private Size resolution;
        private Graphics graphics;
        private BufferedGraphicsContext context;
        private BufferedGraphics buffer;
        private Bitmap image;

        private Mesh mesh;
        private Texture texture;

        public Renderer(Graphics g, Size res, int downScale)
        {
            resolution = res;
            graphics = g;
            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(res.Width + 1, res.Height + 1);
            buffer = context.Allocate(g, new Rectangle(0, 0, res.Width, res.Height));
            image = new Bitmap(res.Width / downScale, res.Height / downScale, PixelFormat.Format24bppRgb);

            // TestCode
            mesh = new Mesh(0,
                new List<Vertex>
                {
                    new Vertex(new Vector4(50f, 50f, 0, 1), new Vector2(0.125f, 0.125f), "red"),
                    new Vertex(new Vector4(100f, 50f, 0, 1), new Vector2(0.25f, 0.125f), "green"),
                    new Vertex(new Vector4(100f, 100f, 0, 1), new Vector2(0.25f, 0.25f), "blue"),
                    new Vertex(new Vector4(50f, 100f, 0, 1), new Vector2(0.125f, 0.25f), "white"),
                },
                new List<int>
                {
                    0, 1, 2,
                    0, 2, 3,
                });

            texture = new Texture("CKMan.png");
        }

        public void Update()
        {

        }

        public void Render()
        {
            var l = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
            texture.Lock();
            mesh.Draw(l, new Matrix4x4(), texture);
            texture.Unlock();
            image.UnlockBits(l);
            buffer.Graphics.DrawImage(image, 0, 0, resolution.Width, resolution.Height);
            buffer.Render(graphics);
        }
    }
}
