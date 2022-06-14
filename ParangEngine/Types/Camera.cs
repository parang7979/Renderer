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
                pMat = Matrix4x4.CreatePerspectiveFieldOfView(Fov.ToRad(), Screen.AspectRatio, Screen.NearPlane, Screen.FarPlane);
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
            foreach (var l in lights) l.Setup(pvMat);
            gBuffer.Render(Screen, ClearColor, lights);
        }

        public Image GetBuffer(GBuffer.BufferType type)
        {
            return gBuffer.GetBuffer(type);
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
    }
}
