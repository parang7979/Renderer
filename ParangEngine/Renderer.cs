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
        private Transform transform3;
        private List<Light> lights;

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
            transform2 = new Transform();
            transform2.Position = new Vector3(3f, 0f, 2f);
            // transform2.Rotation = new Vector3(45f, 0f, 0f);

            transform3 = new Transform();
            transform3.Position = new Vector3(-3f, 0f, 2f);
            // transform3.Rotation = new Vector3(-45f, 0f, 0f);

            lights = new List<Light>();
            var pointLight = new PointLight(new Transform());
            pointLight.Color = new Types.Color(System.Drawing.Color.Red);
            pointLight.Radius = 4f;
            pointLight.Transform.Position = new Vector3(-2f, 1f, 0f);
            lights.Add(pointLight);

            pointLight = new PointLight(new Transform());
            pointLight.Color = new Types.Color(System.Drawing.Color.Green);
            pointLight.Radius = 4f;
            pointLight.Transform.Position = new Vector3(2f, 1f, 0f);
            lights.Add(pointLight);

            pointLight = new PointLight(new Transform());
            pointLight.Color = new Types.Color(System.Drawing.Color.Blue);
            pointLight.Radius = 6f;
            pointLight.Transform.Position = new Vector3(0f, 0f, -2f);
            lights.Add(pointLight);

            pointLight = new PointLight(new Transform());
            pointLight.Color = new Types.Color(System.Drawing.Color.Cyan);
            pointLight.Radius = 6f;
            pointLight.Transform.Position = new Vector3(0f, 2f, 3f);
            lights.Add(pointLight);

            var dirLight = new DirectionalLight(new Transform());
            dirLight.Color = new Types.Color(System.Drawing.Color.Yellow);
            dirLight.Intensity = 0.5f;
            lights.Add(dirLight);

            Gizmos.CreateGrid(10);
        }

        public void Update()
        {
            var rot = transform.Rotation;
            rot.Y -= 1;
            transform.Rotation = rot;
            
            rot = transform2.Rotation;
            rot.Y += 1;
            transform2.Rotation = rot;

            rot = transform3.Rotation;
            rot.Y -= 1;
            transform3.Rotation = rot;

            /* rot = lights[0].Transform.Rotation;
            rot.X -= 1;
            lights[0].Transform.Rotation = rot; */

            /* var pos = lights[0].Transform.Position;
            pos.X = (pos.X + 0.1f) % 10;
            lights[0].Transform.Position = pos; */

            camera.Transform.Update();
            transform.Update();
            transform2.Update();
            transform3.Update();
            foreach (var l in lights) l.Transform.Update();
        }

        public Vertex DefaultVS(Vertex v, Matrix4x4 m)
        {
            return Vertex.Transform(v, m);
        }

        public void Render()
        {
            // 카메라 그리 준비
            camera.Lock();
            // 게임 오브젝트 그룹 단위
            {
                // camera.DrawGrid();
                // 텍스쳐 읽기 준비
                texture.Lock();
                {
                    camera.DrawMesh(mesh, transform, texture, DefaultVS);
                    camera.DrawMesh(mesh, transform2, texture, DefaultVS);
                    camera.DrawMesh(mesh, transform3, texture, DefaultVS);
                }
                texture.Unlock();
            }
            camera.Render(lights);
            camera.Unlock();

            buffer.Graphics.DrawImage(camera.RenderTarget, 0, 0, resolution.Width, resolution.Height);                
            buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Albedo), 0, 0, resolution.Width / 5, resolution.Height / 5);
            buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Position), 0, resolution.Height / 5, resolution.Width / 5, resolution.Height / 5);
            buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Normal), 0, 2 * resolution.Height / 5, resolution.Width / 5, resolution.Height / 5);
            buffer.Render(graphics);
        }
    }
}
