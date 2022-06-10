﻿using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class Camera
    {
        static private readonly Vector4 center = new Vector4(Vector3.Zero, 1f);
        static private readonly Vector4 xAxis = new Vector4(Vector3.UnitX, 1f);
        static private readonly Vector4 yAxis = new Vector4(Vector3.UnitY, 1f);
        static private readonly Vector4 zAxis = new Vector4(Vector3.UnitZ, 1f);

        public Transform Transform { get; private set; } = new Transform();
        public Screen Screen { get; private set; }
        public float Fov { get; set; } = 60f;
        public Color ClearColor { get; set; } = Color.Black;
        
        public Bitmap RenderTarget { get; set; }

        private BitmapData locked;
        private Matrix4x4 vMat;
        private Matrix4x4 pMat;
        private Matrix4x4 pvMat;
        private Frustum frustum;

        public Camera(int width, int height, float fov)
        {
            Screen = new Screen(width, height);
            Fov = fov;
            RenderTarget = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        public void Lock()
        {
            if (locked == null)
            {
                // 뷰 메트릭스
                vMat = Matrix4x4.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);
                // 투영 매트릭스
                pMat = Matrix4x4.CreatePerspectiveFieldOfView(Fov.ToRad(), Screen.AspectRatio, 1f, 100f);
                // 매트릭스 합침
                pvMat = vMat * pMat;
                // 절두체
                frustum = new Frustum(pMat);
                // 렌더 타겟 잠금
                locked = RenderTarget.LockBits(new Rectangle(0, 0, RenderTarget.Width, RenderTarget.Height), ImageLockMode.ReadWrite, RenderTarget.PixelFormat);
                // 컬러 클리어
                locked.Clear(ClearColor);
                DrawGizemos();
            }
        }

        public bool DrawCheck(Transform transform)
        {
            var viewPos = Vector4.Transform(new Vector4(transform.Position, 1), vMat);
            return frustum.Check(viewPos.ToVector3()) != Frustum.Result.Outside;
        }

        public void Render(List<Vertex> vertices, in Transform transform, in Texture texture, Func<Vertex, Matrix4x4, Vertex> VS)
        {
            if (locked != null)
            {
                // 버텍스 변환 L -> V
                var mat = transform.Mat * pvMat;
                for(int i = 0; i < vertices.Count; i++)
                    vertices[i] = VS(vertices[i], mat);

                for (int i = 0; i < vertices.Count / 3; i++)
                {
                    var tri = vertices.GetRange(i * 3, 3);
                }

                CovertToView(vertices);
                // 클립


                // View to NDC
                ConvertToNDC(vertices);

                var edge1 = (v2.Pos - v1.Pos).ToVector3();
                var edge2 = (v3.Pos - v1.Pos).ToVector3();
                // cw backface culling
                var faceNormal = -Vector3.Cross(edge1, edge2);
                if (Vector3.Dot(faceNormal, Vector3.UnitZ) >= 0f)
                    return;

                // NDC to Screen
                v1 = ApplyScreen(v1);
                v2 = ApplyScreen(v2);
                v3 = ApplyScreen(v3);

                // 폴리곤
                locked.DrawPolygon(Screen, v1, v2, v3, texture);
                // 와이어프레임
                locked.DrawLine(Screen, v1, v2);
                locked.DrawLine(Screen, v2, v3);
                locked.DrawLine(Screen, v3, v1);
            }
        }

        public void RenderAxes(in Transform transform)
        {
            if (locked != null)
            {
                var c = ApplyScreen(ConvertToNDC(CovertToView(center * transform)));
                var x = ApplyScreen(ConvertToNDC(CovertToView(xAxis * transform)));
                var y = ApplyScreen(ConvertToNDC(CovertToView(yAxis * transform)));
                var z = ApplyScreen(ConvertToNDC(CovertToView(zAxis * transform)));
                locked.DrawLine(Screen, c, x, new Color("red"));
                locked.DrawLine(Screen, c, y, new Color("green"));
                locked.DrawLine(Screen, c, z, new Color("blue"));
            }
        }

        private void DrawGizemos()
        {
            if (Gizmos.Grids != null)
            {
                foreach (var (p1, p2) in Gizmos.Grids)
                {
                    Vector4 v1 = p1;
                    Vector4 v2 = p2;

                    v1 = CovertToView(v1);
                    v2 = CovertToView(v2);

                    v1 = v1.Clip(v2);
                    v2 = v2.Clip(v1);

                    // View to NDC
                    v1 = ConvertToNDC(v1);
                    v2 = ConvertToNDC(v2);

                    v1 = ApplyScreen(v1);
                    v2 = ApplyScreen(v2);

                    locked.DrawLine(Screen, v1, v2, new Color("gray"));
                }
            }
        }

        public void Unlock()
        {
            if (locked != null)
            {
                RenderTarget.UnlockBits(locked);
                locked = null;
            }
        }

        private void CovertToView(List<Vertex> vertices)
        {
            for(int i = 0; i < vertices.Count; i++)
                vertices[i] = CovertToView(vertices[i]);
        }

        private Vertex CovertToView(Vertex v)
        {
            v.Pos = CovertToView(v.Pos);
            return v;
        }

        private Vector4 CovertToView(Vector4 v)
        {
            v = Vector4.Transform(v, vpMat);
            return v;
        }

        private void ConvertToNDC(List<Vertex> vertices)
        {
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = ConvertToNDC(vertices[i]);
        }

        private Vertex ConvertToNDC(Vertex v)
        {
            v.Pos = ConvertToNDC(v.Pos);
            return v;
        }

        private Vector4 ConvertToNDC(Vector4 v)
        {
            v.W = v.W == 0f ? float.Epsilon : v.W;
            var invW = 1f / v.W;
            v.X *= invW;
            v.Y *= invW;
            v.Z *= invW;
            return v;
        }

        private void ApplyScreen(List<Vertex> vertices)
        {
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = ApplyScreen(vertices[i]);
        }

        private Vertex ApplyScreen(Vertex v)
        {
            v.Pos = ApplyScreen(v.Pos);
            return v;
        }

        private Vector4 ApplyScreen(Vector4 v)
        {
            v.X *= Screen.HalfWidth;
            v.Y *= Screen.HalfHeight;
            return v;
        }
    }
}
