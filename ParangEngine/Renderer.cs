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

            // TestCode
            mesh = new Mesh(0,
                new List<Vertex>
                {
                    new Vertex(new Vector4(-0.5f, 0.5f, 0.5f, 1), new Vector2(0.25f, 0.125f), "cyan"),
                    new Vertex(new Vector4(0.5f, 0.5f, 0.5f, 1), new Vector2(0.125f, 0.125f), "magenta"),
                    new Vertex(new Vector4(0.5f, -0.5f, 0.5f, 1), new Vector2(0.125f, 0.25f), "yellow"),
                    new Vertex(new Vector4(-0.5f, -0.5f, 0.5f, 1), new Vector2(0.25f, 0.25f), "black"),
                    new Vertex(new Vector4(-0.5f, 0.5f, -0.5f, 1), new Vector2(0.375f, 0.125f), "red"),
                    new Vertex(new Vector4(0.5f, 0.5f, -0.5f, 1), new Vector2(0f, 0.25f), "green"),
                    new Vertex(new Vector4(0.5f, -0.5f, -0.5f, 1), new Vector2(0f, 0.125f), "blue"),
                    new Vertex(new Vector4(-0.5f, -0.5f, -0.5f, 1), new Vector2(0.375f, 0.25f), "white"),
                },
                new List<int>
                {
                    0, 3, 2, 0, 2, 1,
                    1, 2, 6, 1, 6, 5,
                    4, 7, 3, 4, 3, 0,
                });
            texture = new Texture(0, "CKMan.png");
            transform = new Transform();
            transform.Position = new Vector3(0f, 0f, 0f);
            transform2 = new Transform();
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
                // 텍스쳐 읽기 준비
                texture.Lock();
                {
                    mesh.Render(camera, transform, texture);
                    // mesh.Render(camera, transform2, texture);
                }
                texture.Unlock();
            }
            camera.Unlock();
            buffer.Graphics.DrawImage(camera.RenderTarget, 0, 0, resolution.Width, resolution.Height);
            buffer.Render(graphics);
        }
    }
}
