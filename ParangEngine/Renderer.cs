﻿using ParangEngine.Types;
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

        private List<GameObject> objects;

        private Camera camera;

        private Mesh mesh;
        private Material material;
        private Material material2;

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
            camera.Transform = new Transform();
            camera.Transform.Rotation = new Vector3(45f, 0f, 0f);
            camera.Transform.Position = new Vector3(0f, 2.5f, -2.5f);
            // camera.Transform.Rotation = new Vector3(0f, 0f, 0f);

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
            mesh = new Mesh(0, v.ToList(), i);
            material = new Material(0);
            material.AddTexture(Material.Type.Albedo, new Texture("wall1_color.png"));
            material.AddTexture(Material.Type.Normal, new Texture("wall1_n.png"));

            material2 = new Material(1);
            material2.AddTexture(Material.Type.Albedo, new Texture("rock_diffuse.png"));
            material2.AddTexture(Material.Type.Normal, new Texture("rock_normal.png"));

            transform = new Transform();
            transform.Position = new Vector3(0f, 0f, 0f);
            transform.Rotation = new Vector3(0f, 180f, 0f);

            transform2 = new Transform();
            transform2.Position = new Vector3(2f, 1f, 1f);
            transform2.Rotation = new Vector3(0f, 180f, 0f);

            transform3 = new Transform();
            transform3.Position = new Vector3(-2f, -1f, 0f);
            transform3.Rotation = new Vector3(0f, 180f, 0f);

            lights = new List<Light>();
            var dirLight = new DirectionalLight();
            dirLight.Transform = new Transform();
            dirLight.Color = new Types.Color(System.Drawing.Color.Yellow);
            var rot = dirLight.Transform.Rotation;
            rot = new Vector3(0f, 0f, 90f);
            dirLight.Transform.Rotation = rot;
            dirLight.Intensity = 0.5f;
            lights.Add(dirLight);

            dirLight = new DirectionalLight();
            dirLight.Transform = new Transform();
            dirLight.Color = new Types.Color(System.Drawing.Color.SkyBlue);
            rot = dirLight.Transform.Rotation;
            rot = new Vector3(45f, 45f, 0f);
            dirLight.Transform.Rotation = rot;
            dirLight.Intensity = 0.5f;
            lights.Add(dirLight);

            var pointLight = new PointLight();
            pointLight.Transform = new Transform();
            pointLight.Color = new Types.Color(System.Drawing.Color.White);
            pointLight.Intensity = 5f;
            pointLight.Radius = 5f;
            pointLight.Transform.Position = new Vector3(0f, 0f, -2f);
            lights.Add(pointLight);

            pointLight = new PointLight();
            pointLight.Transform = new Transform();
            pointLight.Color = new Types.Color(System.Drawing.Color.Green);
            pointLight.Intensity = 6f;
            pointLight.Radius = 6f;
            pointLight.Transform.Position = new Vector3(0f, 0f, 2f);
            lights.Add(pointLight);

            pointLight = new PointLight();
            pointLight.Transform = new Transform();
            pointLight.Color = new Types.Color(System.Drawing.Color.Red);
            pointLight.Intensity = 6f;
            pointLight.Radius = 6f;
            pointLight.Transform.Position = new Vector3(0f, 2f, 0f);
            lights.Add(pointLight);

            pointLight = new PointLight();
            pointLight.Transform = new Transform();
            pointLight.Color = new Types.Color(System.Drawing.Color.Blue);
            pointLight.Intensity = 4f;
            pointLight.Radius = 4f;
            pointLight.Transform.Position = new Vector3(-1f, 1f, -1f);
            lights.Add(pointLight);

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

            rot = lights[0].Transform.Rotation;
            rot.X += 2;
            lights[0].Transform.Rotation = rot;

            /* var pos = lights[1].Transform.Position;
            pos.X = (pos.X + 0.1f) % 10;
            lights[1].Transform.Position = pos; */

            camera.Transform.Update();
            transform.Update();
            transform2.Update();
            transform3.Update();
            foreach (var l in lights)
            {
                l.Transform.Update();
                l.Update();
            }
        }

        public void DrawMesh()
        {
            // 카메라 그리 준비
            camera.Lock();
            // 게임 오브젝트 그룹 단위
            {
                camera.DrawGrid();
                // 텍스쳐 읽기 준비
                material.Lock();
                {
                    camera.DrawMesh(transform, mesh, material);
                }
                material.Unlock();

                material2.Lock();
                {
                    camera.DrawMesh(transform2, mesh, material2);
                    camera.DrawMesh(transform3, mesh, material2);
                }
                material2.Unlock();
            }
            camera.Unlock();
        }

        public void DrawRender()
        {
            camera.Render(lights);
            buffer.Graphics.DrawImage(camera.RenderTarget, 0, 0, resolution.Width, resolution.Height);
            /* buffer.Graphics.DrawImage(camera.RenderTarget, 0, 0, resolution.Width / 2, resolution.Height / 2);
            buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Albedo), resolution.Width / 2, 0, resolution.Width / 2, resolution.Height / 2);
            buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Position), 0, resolution.Height / 2, resolution.Width / 2, resolution.Height / 2);
            buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Normal), resolution.Width / 2, resolution.Height / 2, resolution.Width / 2, resolution.Height / 2); */
            // buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Specular), 0, 3 * resolution.Height / 5, resolution.Width / 5, resolution.Height / 5); */
            buffer.Render(graphics);
        }

        public void Render()
        {
            camera.SwitchBuffer();
            var task1 = Task.Factory.StartNew(DrawMesh);
            var task2 = Task.Factory.StartNew(DrawRender);
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
