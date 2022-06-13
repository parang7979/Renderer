using ParangEngine.Utils;
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
    public partial class Camera
    {
        public Transform Transform { get; private set; } = new Transform();
        public Screen Screen { get; private set; }
        public float Fov { get; set; } = 60f;
        public Color ClearColor { get; set; } = Color.Black;
        public Image RenderTarget => gBuffer.RenderTarget;

        private GBuffer gBuffer;
        private Matrix4x4 vMat;
        private Matrix4x4 pMat;
        private Matrix4x4 pvMat;
        private Frustum frustum;

        public Camera(int width, int height, float fov)
        {
            Screen = new Screen(width, height);
            Fov = fov;
            gBuffer = new GBuffer(width, height);
        }

        public void Lock()
        {
            if (!gBuffer.IsLock)
            {
                // 뷰 메트릭스
                vMat = Matrix4x4.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);
                // 투영 매트릭스
                pMat = Matrix4x4.CreatePerspectiveFieldOfView(Fov.ToRad(), Screen.AspectRatio, 1f, 100f);
                // 매트릭스 합침
                pvMat = vMat * pMat;
                // 절두체
                frustum = new Frustum(pMat);
                // 버퍼 락
                gBuffer.Lock();
            }
        }

        public void Unlock()
        {
            if (!gBuffer.IsLock) return;
            gBuffer.Unlock();
        }

        public void Render(List<Light> lights)
        {
            if (!gBuffer.IsLock) return;
            foreach (var l in lights)
            {
                l.Setup(pvMat);
                var v = new Vertex(Vector3.Zero, 1, "red");
                gBuffer.DrawCircle(Screen, 
                    ConvertToNDC(Vertex.Transform(v, l.Transform.Mat * pvMat)), 
                    l.Intensity * 30f, 
                    l.Color);
                DrawAxes(l.Transform);
            }
            gBuffer.Render(ClearColor, lights);
        }

        public Image GetBuffer(GBuffer.BufferType type)
        {
            return gBuffer.GetBuffer(type);
        }

        private void ConvertToNDC(List<Vertex> vertices)
        {
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = ConvertToNDC(vertices[i]);
        }

        private Vertex ConvertToNDC(Vertex v)
        {
            v.W = v.W == 0f ? float.Epsilon : v.W;
            var invW = 1f / v.W;
            v.X *= invW;
            v.Y *= invW;
            v.Z *= invW;
            return v;
        }

        private bool BackfaceCulling(List<Vertex> vertices)
        {
            var edge1 = vertices[1].Vector3 - vertices[0].Vector3;
            var edge2 = vertices[2].Vector3 - vertices[0].Vector3;
            // cw backface culling
            var faceNormal = -Vector3.Cross(edge1, edge2);
            if (Vector3.Dot(faceNormal, Vector3.UnitZ) >= 0f)
                return false;
            return true;
        }

        private void ApplyScreen(List<Vertex> vertices)
        {
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = ApplyScreen(vertices[i]);
        }

        private Vertex ApplyScreen(Vertex v)
        {
            v.X *= Screen.HalfWidth;
            v.Y *= Screen.HalfHeight;
            return v;
        }
    }
}
