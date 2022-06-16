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
            camera.Transform.Rotation = new Vector3(0f, 0f, 0f);
            camera.Transform.Position = new Vector3(0f, 0f, -5f);
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
                new Vertex(-1.0f, 1.0f, -1.0f, 1f), new Vertex(1.0f, 1.0f, -1.0f, 1f), new Vertex(1.0f, 1.0f, 1.0f, 1f), new Vertex(-1.0f, 1.0f, 1.0f, 1f),
                new Vertex(-1.0f, -1.0f, -1.0f, 1f), new Vertex(1.0f, -1.0f, -1.0f, 1f), new Vertex(1.0f, -1.0f, 1.0f, 1f), new Vertex(-1.0f, -1.0f, 1.0f, 1f)
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
            mesh = new Mesh(0, v.ToList(), i);
            texture = new Texture(0, "wall1_n.png");
            transform = new Transform();
            transform.Position = new Vector3(0f, 0f, 3f);
            transform.Rotation = new Vector3(0f, 180f, 0f);

            transform2 = new Transform();
            transform2.Position = new Vector3(5f, 3f, 10f);
            transform2.Rotation = new Vector3(0f, 180f, 0f);

            transform3 = new Transform();
            transform3.Position = new Vector3(-3f, -1f, 0f);
            transform3.Rotation = new Vector3(0f, 180f, 0f);

            lights = new List<Light>();
            /*var pointLight = new PointLight(new Transform());
            pointLight.Color = new Types.Color(System.Drawing.Color.Blue);
            pointLight.Radius = 4f;
            pointLight.Transform.Position = new Vector3(-4f, 0f, -1f);
            lights.Add(pointLight);

            pointLight = new PointLight(new Transform());
            pointLight.Color = new Types.Color(System.Drawing.Color.Green);
            pointLight.Radius = 4f;
            pointLight.Transform.Position = new Vector3(4f, 0f, -1f);
            lights.Add(pointLight);*/

            var pointLight = new PointLight(new Transform());
            pointLight.Color = new Types.Color(System.Drawing.Color.White);
            pointLight.Intensity = 10f;
            pointLight.Radius = 10f;
            pointLight.Transform.Position = new Vector3(0f, 0f, 8f);
            lights.Add(pointLight);

            /* pointLight = new PointLight(new Transform());
            pointLight.Color = new Types.Color(System.Drawing.Color.Blue);

            pointLight = new PointLight(new Transform());
            pointLight.Color = new Types.Color(System.Drawing.Color.Green);
            pointLight.Radius = 6f;
            pointLight.Transform.Position = new Vector3(0f, 0f, -2f);
            lights.Add(pointLight);

            pointLight = new PointLight(new Transform());
            pointLight.Color = new Types.Color(System.Drawing.Color.Green);
            pointLight.Radius = 6f;
            pointLight.Transform.Position = new Vector3(0f, 2f, 3f);
            lights.Add(pointLight); */

            /*var dirLight = new DirectionalLight(new Transform());
            dirLight.Color = new Types.Color(System.Drawing.Color.White);
            var rot = dirLight.Transform.Rotation;
            rot = new Vector3(0f, 0f, 0f);
            dirLight.Transform.Rotation = rot;
            dirLight.Intensity = 1f;
            lights.Add(dirLight); */

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

            /* var rot = lights[0].Transform.Rotation;
            rot.Y -= 2;
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

        public Vertex DefaultVS(Vertex v, Matrix4x4 mat, Matrix4x4 pvMat)
        {
            v = Vertex.Transform(v, mat);
            v = Vertex.TransformNormal(v, mat);
            return Vertex.Transform(v, pvMat);
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
                    // camera.DrawMesh(mesh, transform, texture, new Types.Color("white"), DefaultVS);
                    camera.DrawMesh(mesh, transform2, texture, new Types.Color("white"), DefaultVS);
                    // camera.DrawMesh(mesh, transform3, texture, new Types.Color("blue"), DefaultVS);
                }
                texture.Unlock();
            }
            camera.Render(lights);
            camera.Unlock();

            buffer.Graphics.DrawImage(camera.RenderTarget, 0, 0, resolution.Width, resolution.Height);                
            buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Albedo), 0, 0, resolution.Width / 5, resolution.Height / 5);
            buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Position), 0, resolution.Height / 5, resolution.Width / 5, resolution.Height / 5);
            buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Normal), 0, 2 * resolution.Height / 5, resolution.Width / 5, resolution.Height / 5);
            // buffer.Graphics.DrawImage(camera.GetBuffer(GBuffer.BufferType.Specular), 0, 3 * resolution.Height / 5, resolution.Width / 5, resolution.Height / 5); */
            buffer.Render(graphics);
        }
    }
}
