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
        private Graphics graphics;
        private BufferedGraphicsContext context;
        private BufferedGraphics buffer;
        
        private Camera camera;
        private Mesh mesh;
        private Texture texture;
        private Transform transform;
        private Transform transform2;

        public Renderer(Graphics g, Size res, int downScale)
        {
            resolution = res;
            graphics = g;
            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(res.Width + 1, res.Height + 1);
            buffer = context.Allocate(g, new Rectangle(0, 0, res.Width, res.Height));

            camera = new Camera(res.Width / downScale, res.Height / downScale, 60f);
            camera.Transform.Rotation = new Vector3(45f, 0f, 0f);
            camera.Transform.Position = new Vector3(0f, 5f, -5f);
            // camera.Transform.Rotation = new Vector3(0f, 0f, 0f);

            var v = new List<Vector3>()
            {
                new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, -1.0f, 1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, -1.0f),
                new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, -1.0f, 1.0f), new Vector3(-1.0f, -1.0f, 1.0f)
            };

            var i = new List<int>()
            {
                0, 1, 2, 0, 2, 3, // Right
	            4, 6, 5, 4, 7, 6, // Front
	            8, 9, 10, 8, 10, 11, // Back
	            12, 14, 13, 12, 15, 14, // Left
	            16, 18, 17, 16, 19, 18, // Top
	            20, 21, 22, 20, 22, 23  // Bottom
            };

            // TestCode
            mesh = new Mesh(0, v.Select(x => new Vertex(x, 1, color : "white")).ToList(), i);
            texture = new Texture(0, "CKMan.png");
            transform = new Transform();
            transform.Rotation = new Vector3(0f, 180f, 0f);
            transform2 = new Transform();
            Gizmos.CreateGrid(10);
        }

        public void Update()
        {
            var rot = transform.Rotation;
            rot.Y += 1;
            transform.Rotation = rot;

            camera.Transform.Update();
            transform.Update();
            transform2.Update();
        }

        public void Render()
        {
            // 카메라 그리 준비
            camera.Lock();
            // 게임 오브젝트 그룹 단위
            {
                camera.DrawGrid();
                // 텍스쳐 읽기 준비
                texture.Lock();
                {
                    if (camera.DrawCheck(transform))
                    {
                        mesh.Render(camera, transform, texture);
                    }
                }
                texture.Unlock();
            }
            var gBuffer = camera.Render();
            camera.Unlock();

            if (gBuffer != null)
            {
                buffer.Graphics.DrawImage(gBuffer.RenderTarget, 0, 0, resolution.Width, resolution.Height);                
                /* buffer.Graphics.DrawImage(gBuffer.GetBuffer(GBuffer.BufferType.Albedo), 0, 0, resolution.Width / 5, resolution.Height / 5);
                buffer.Graphics.DrawImage(gBuffer.GetBuffer(GBuffer.BufferType.Position), 0, resolution.Height / 5, resolution.Width / 5, resolution.Height / 5);
                buffer.Graphics.DrawImage(gBuffer.GetBuffer(GBuffer.BufferType.Normal), 0, 2 * resolution.Height / 5, resolution.Width / 5, resolution.Height / 5); */
                buffer.Render(graphics);
            }
        }
    }
}
