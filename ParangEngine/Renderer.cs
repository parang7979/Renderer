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
        
        private Dictionary<long, Mesh> meshs = new Dictionary<long, Mesh>();
        private Dictionary<long, Material> materials = new Dictionary<long, Material>();
        private Dictionary<long, Texture> textures = new Dictionary<long, Texture>();

        private Scene scene;

        public Renderer(Graphics g, Size res, int downScale)
        {
            resolution = res;
            graphics = g;
            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(res.Width + 1, res.Height + 1);
            buffer = context.Allocate(g, new Rectangle(0, 0, res.Width, res.Height));

            var v = new List<Vertex>()
            {
                new Vertex(-1.0f, -1.0f, -1.0f, 1f), new Vertex(-1.0f, -1.0f, 1.0f, 1f), new Vertex(-1.0f, 1.0f, 1.0f, 1f), new Vertex(-1.0f, 1.0f, -1.0f, 1f),
                new Vertex(-1.0f, -1.0f, 1.0f, 1f, uv: new Vector2(1f, 1f)), 
                new Vertex(-1.0f, 1.0f, 1.0f, 1f, uv: new Vector2(1f, 0f)),
                new Vertex(1.0f, 1.0f, 1.0f, 1f, uv: new Vector2(0f, 0f)),
                new Vertex(1.0f, -1.0f, 1.0f, 1f, uv: new Vector2(0f, 1f)),
                new Vertex(-1.0f, -1.0f, -1.0f, 1f), new Vertex(-1.0f, 1.0f, -1.0f, 1f), new Vertex(1.0f, 1.0f, -1.0f, 1f), new Vertex(1.0f, -1.0f, -1.0f, 1f),
                new Vertex(1.0f, -1.0f, -1.0f, 1f), new Vertex(1.0f, -1.0f, 1.0f, 1f), new Vertex(1.0f, 1.0f, 1.0f, 1f), new Vertex(1.0f, 1.0f, -1.0f, 1f),
                new Vertex(-1.0f, 1.0f, -1.0f, 1f, uv: new Vector2(0f, 0f)), //16
                new Vertex(1.0f, 1.0f, -1.0f, 1f, uv: new Vector2(1f, 0f)),  //17
                new Vertex(1.0f, 1.0f, 1.0f, 1f, uv: new Vector2(1f, 1f)), //19
                new Vertex(-1.0f, 1.0f, 1.0f, 1f, uv: new Vector2(0f, 1f)),//19
                new Vertex(-1.0f, -1.0f, -1.0f, 1f), new Vertex(1.0f, -1.0f, -1.0f, 1f), new Vertex(1.0f, -1.0f, 1.0f, 1f), new Vertex(-1.0f, -1.0f, 1.0f, 1f)
            };

            var i = new List<int>()
            {
                // 0, 1, 2, 0, 2, 3, // Right
	            4, 6, 5, 4, 7, 6, // Front
	            /* 8, 9, 10, 8, 10, 11, // Back
	            12, 14, 13, 12, 15, 14, // Left*/
	            16, 18, 17, 16, 19, 18, // Top
	            /* 20, 21, 22, 20, 22, 23  // Bottom */
            };

            // TestCode
            
            // Load Texture
            textures.Add(0, new Texture("wall1_color.png"));
            textures.Add(1, new Texture("wall1_n.png"));
            textures.Add(2, new Texture("rock_diffuse.png"));
            textures.Add(3, new Texture("rock_normal.png"));
            // Load Mesh
            meshs.Add(0, new Mesh(0, v.ToList(), i));
            // create material
            var material = new Material(0);
            material.AddTexture(Material.Type.Albedo, textures[0]);
            material.AddTexture(Material.Type.Normal, textures[1]);

            scene = new Scene();

            // camera
            var cameraGo = new GameObject();
            cameraGo.Transform.Rotation = new Vector3(45f, 0f, 0f);
            cameraGo.Transform.Position = new Vector3(0f, 2.5f, -2.5f);

            var camera = new Camera(res.Width / downScale, res.Height / downScale, 60f);
            cameraGo.AddComponent(camera);
            scene.Add(cameraGo);

            // mesh
            var meshGo = new GameObject();
            meshGo.Transform.Rotation = new Vector3(0f, 180f, 0f);

            meshGo.AddComponent(new MeshRenderer(meshs[0], material));
            scene.Add(meshGo);

            var lightGo = new GameObject();
            var dirLight = new DirectionalLight();
            dirLight.Color = Types.Color.White;
            lightGo.AddComponent(dirLight);
            lightGo.Transform.Rotation = new Vector3(45f, 0f, 0f);
            /* var pointLight = new PointLight();
            pointLight.Color = Types.Color.White;
            pointLight.Intensity = 5f;
            pointLight.Radius = 5f;
            lightGo.AddComponent(pointLight);
            lightGo.Transform.Position = new Vector3(0f, 0f, -2.5f); */
            scene.Add(lightGo);
            Gizmos.CreateGrid(10);
        }

        public void Update()
        {
            scene.Update();
        }

        private void DrawTask()
        {
            scene.Draw();
        }

        private void RenderTask()
        {
            scene.Render();
            buffer.Graphics.DrawImage(scene.MainCamera.RenderTarget, 0, 0, resolution.Width, resolution.Height);
            /* buffer.Graphics.DrawImage(camera.RenderTarget, 0, 0, resolution.Width / 2, resolution.Height / 2);
            buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Albedo), resolution.Width / 2, 0, resolution.Width / 2, resolution.Height / 2);
            buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Position), 0, resolution.Height / 2, resolution.Width / 2, resolution.Height / 2);
            buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Normal), resolution.Width / 2, resolution.Height / 2, resolution.Width / 2, resolution.Height / 2); */
            // buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Specular), 0, 3 * resolution.Height / 5, resolution.Width / 5, resolution.Height / 5); */
            buffer.Render(graphics);
        }

        public void Render()
        {
            scene.SwitchBuffer();
            var task1 = Task.Factory.StartNew(DrawTask);
            var task2 = Task.Factory.StartNew(RenderTask);
            var check1 = false;
            var check2 = false;
            var now = DateTime.UtcNow;
            while (!(task1.IsCompleted && task2.IsCompleted))
            {
                if (!check1 && task1.IsCompleted)
                {
                    check1 = true;
                    Console.WriteLine($"1 : {DateTime.UtcNow - now}");
                }

                if (!check2 && task2.IsCompleted)
                {
                    check2 = true;
                    Console.WriteLine($"2 : {DateTime.UtcNow - now}");
                }
            }
            if (!check1 && task1.IsCompleted)
            {
                check1 = true;
                Console.WriteLine($"1 : {DateTime.UtcNow - now}");
            }

            if (!check2 && task2.IsCompleted)
            {
                check2 = true;
                Console.WriteLine($"2 : {DateTime.UtcNow - now}");
            }
        }
    }
}
