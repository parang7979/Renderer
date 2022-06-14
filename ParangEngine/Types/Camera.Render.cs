using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public partial class Camera
    {
        public void DrawGrid()
        {
            var tr = new Transform();
            tr.Update();
            RenderLine(Gizmos.Grids.ToList(), tr);
        }

        public bool DrawCheck(Transform transform)
        {
            var viewPos = Vector4.Transform(new Vector4(transform.Position, 1), vMat);
            return frustum.Check(viewPos.ToVector3()) != Frustum.Result.Outside;
        }

        public void DrawMesh(in Mesh mesh, in Transform transform, in Texture texture, Func<Vertex, Matrix4x4, Vertex> VS)
        {
            if (!DrawCheck(transform)) return;
            RenderTri(mesh.Vertices, transform, texture, VS);
            DrawAxes(transform);
        }

        private void DrawAxes(in Transform transform)
        {
            RenderLine(Gizmos.Axes.ToList(), transform);
        }

        private void RenderTri(List<Vertex> vertices, in Transform transform, in Texture texture, Func<Vertex, Matrix4x4, Vertex> VS)
        {
            if (gBuffer.IsLock)
            {
                // 버텍스 변환 L -> V
                var mat = transform.Mat * pvMat;
                for (int i = 0; i < vertices.Count; i++)
                    vertices[i] = VS(vertices[i], mat);

                var triCount = vertices.Count / 3;
                for (int i = 0; i < triCount; i++)
                {
                    var tri = vertices.GetRange(i * 3, 3);
                    RenderTriOnce(tri, texture);
                }
            }
        }

        private void RenderTriOnce(List<Vertex> vertices, in Texture texture)
        {
            ClipTriangles.Clip(ref vertices);
            // View to NDC
            Vertex.ToNDC(vertices);
            var triCount = vertices.Count / 3;
            for (int j = 0; j < triCount; j++)
            {
                var tri = vertices.GetRange(j * 3, 3);
                if (!BackfaceCulling(tri)) continue;
                Vertex.ToScreen(tri, Screen);
                gBuffer.DrawTriangle(Screen, tri[0], tri[1], tri[2], texture);
                // gBuffer.DrawWireframe(Screen, tri[0], tri[1], tri[2]);
            }
        }

        private void RenderLine(List<Vertex> vertices, in Transform transform)
        {
            // 버텍스 변환 L -> V
            var mat = transform.Mat * pvMat;
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = Vertex.Transform(vertices[i], mat);

            var lineCount = vertices.Count / 2;
            for (int i = 0; i < lineCount; i++)
            {
                var line = vertices.GetRange(i * 2, 2);
                RenderLineOnce(line);
            }
        }

        private void RenderLineOnce(List<Vertex> vertices)
        {
            ClipLines.Clip(ref vertices);
            // View to NDC
            Vertex.ToNDC(vertices);
            var lineCount = vertices.Count / 2;
            for (int j = 0; j < lineCount; j++)
            {
                var line = vertices.GetRange(j * 2, 2);
                Vertex.ToScreen(line, Screen);
                gBuffer.DrawLine(Screen, line[0], line[1]);
            }
        }
    }
}
