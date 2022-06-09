using ParangEngine.Types;
using ParangEngine.Utils;
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
        private ScreenSize screenSize;
        private Graphics graphics;
        private BufferedGraphicsContext context;
        private BufferedGraphics buffer;
        private Bitmap image;

        private Mesh mesh;
        private Texture texture;
        private Transform transform;
        private Transform transform2;
        private Camera camera;

        public Renderer(Graphics g, Size res, int downScale)
        {
            resolution = res;
            graphics = g;
            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(res.Width + 1, res.Height + 1);
            buffer = context.Allocate(g, new Rectangle(0, 0, res.Width, res.Height));

            image = new Bitmap(res.Width / downScale, res.Height / downScale, PixelFormat.Format24bppRgb);
            screenSize = new ScreenSize(image);

            // TestCode
            mesh = new Mesh(0,
                new List<Vertex>
                {
                    new Vertex(new Vector4(-25f, 25f, 25f, 1), new Vector2(0.125f, 0.125f), "red"),
                    new Vertex(new Vector4(25f, 25f, 25f, 1), new Vector2(0.25f, 0.125f), "green"),
                    new Vertex(new Vector4(25f, -25f, 25f, 1), new Vector2(0.25f, 0.25f), "blue"),
                    new Vertex(new Vector4(-25f, -25f, 25f, 1), new Vector2(0.125f, 0.25f), "white"),
                    
                    new Vertex(new Vector4(-25f, 25f, -25f, 1), new Vector2(0f, 0.125f), "cyan"),
                    new Vertex(new Vector4(25f, 25f, -25f, 1), new Vector2(0.375f, 0.125f), "magenta"),
                    new Vertex(new Vector4(25f, -25f, -25f, 1), new Vector2(0.375f, 0.25f), "yellow"),
                    new Vertex(new Vector4(-25f, -25f, -25f, 1), new Vector2(0f, 0.25f), "black"),
                },
                new List<int>
                {
                    0, 1, 3, 1, 3, 2,
                    1, 5, 2, 5, 2, 6,
                    // 5, 4, 6, 4, 6, 7,
                    4, 0, 7, 0, 7, 3,
                    // 4, 5, 0, 5, 0, 1,
                    // 3, 2, 7, 2, 7, 6,
                });

            texture = new Texture("CKMan.png");
            transform = new Transform();
            var pos = transform.Position;
            // pos.X += 100;
            // pos.Y += 10;
            transform.Position = pos;

            transform2 = new Transform();
            var pos2 = transform2.Position;
            pos2.X += 100;
            // pos2.Y += 100;
            transform2.Position = pos2;

            camera = new Camera();
            camera.Transform.Rotation = new Vector3(45f, 180f, 0f);
            // camera.Transform.Position = new Vector3(-160f, -120f, 0f);
        }

        public void Update()
        {
            /* var pos = transform.Position;
            pos.X += 10;
            pos.X %= 100;
            transform.Position = pos; */
            var rot = transform.Rotation;
            rot.Y += 1f;
            transform.Rotation = rot;
            
            
        }

        public void Render()
        {
            var l = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
            l.Clear(Types.Color.Black);
            texture.Lock();
            mesh.Draw(l, screenSize, camera, transform, texture);
            // mesh.Draw(l, screenSize, camera, transform2, texture);
            texture.Unlock();
            image.UnlockBits(l);
            buffer.Graphics.DrawImage(image, 0, 0, resolution.Width, resolution.Height);
            buffer.Render(graphics);
        }
    }
}
