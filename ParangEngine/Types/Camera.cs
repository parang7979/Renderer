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
    public class Camera
    {
        static private readonly List<Vertex> axes = new List<Vertex>{
            new Vertex(Vector3.Zero, 1f, "white"),
            new Vertex(Vector3.UnitX, 1f, "red"),
            new Vertex(Vector3.UnitY, 1f, "green"),
            new Vertex(Vector3.UnitZ, 1f, "blue"),
        };

        public Transform Transform { get; private set; } = new Transform();
        public Screen Screen { get; private set; }
        public float Fov { get; set; } = 60f;
        public Color ClearColor { get; set; } = Color.Black;

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

        public bool DrawCheck(Transform transform)
        {
            var viewPos = Vector4.Transform(new Vector4(transform.Position, 1), vMat);
            return frustum.Check(viewPos.ToVector3()) != Frustum.Result.Outside;
        }

        public void RenderTri(List<Vertex> vertices, in Transform transform, in Texture texture, Func<Vertex, Matrix4x4, Vertex> VS)
        {
            if (gBuffer.IsLock)
            {
                // 복사본 생성
                var vs = vertices.ToList();

                // 버텍스 변환 L -> V
                var mat = transform.Mat * pvMat;
                for(int i = 0; i < vs.Count; i++)
                    vs[i] = VS(vs[i], mat);

                var triCount = vs.Count / 3;
                for (int i = 0; i < triCount; i++)
                {
                    var tri = vs.GetRange(i * 3, 3);
                    RenderTriOnce(tri, texture);
                }
            }
        }

        private void RenderTriOnce(List<Vertex> vertices, in Texture texture)
        {
            ClipTriangles.Clip(ref vertices);
            // View to NDC
            ConvertToNDC(vertices);
            var triCount = vertices.Count / 3;
            for (int j = 0; j < triCount; j++)
            {
                var tri = vertices.GetRange(j * 3, 3);
                if (!BackfaceCulling(tri)) continue;
                ApplyScreen(tri);
                gBuffer.DrawTriangle(Screen, tri[0], tri[1], tri[2], texture);
                gBuffer.DrawWireframe(Screen, tri[0], tri[1], tri[2]);
            }
        }

        public void DrawGrid()
        {
            var tr = new Transform();
            tr.Update();
            RenderLine(Gizmos.Grids, tr);
        }

        public void DrawAxes(in Transform transform)
        {
            var vertices = new List<Vertex>()
                {
                    axes[0],
                    axes[1],
                    axes[0],
                    axes[2],
                    axes[0],
                    axes[3],
                };
            RenderLine(vertices, transform);
        }

        private void RenderLine(List<Vertex> vertices, in Transform transform)
        {
            // 복사본 생성
            var vs = vertices.ToList();

            // 버텍스 변환 L -> V
            var mat = transform.Mat * pvMat;
            for (int i = 0; i < vs.Count; i++)
                vs[i] = Vertex.Transform(vs[i], mat);

            var lineCount = vs.Count / 2;
            for (int i = 0; i < lineCount; i++)
            {
                var line = vs.GetRange(i * 2, 2);
                RenderLineOnce(line);
            }
        }

        private void RenderLineOnce(List<Vertex> vertices)
        {
            ClipLines.Clip(ref vertices);
            // View to NDC
            ConvertToNDC(vertices);
            var lineCount = vertices.Count / 2;
            for (int j = 0; j < lineCount; j++)
            {
                var line = vertices.GetRange(j * 2, 2);
                ApplyScreen(line);
                gBuffer.DrawLine(Screen, line[0], line[1]);
            }
        }

        public void Unlock()
        {
            if (!gBuffer.IsLock) return;
            gBuffer.Unlock();
        }

        public GBuffer Render()
        {
            if (!gBuffer.IsLock) return null;
            gBuffer.Render(ClearColor);
            return gBuffer;
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
