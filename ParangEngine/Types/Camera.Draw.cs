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
        private GBuffer drawBuffer => gBuffers[current];

        public void Lock()
        {
            if (!drawBuffer.IsLock)
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
                drawBuffer.Lock(true);
            }
        }

        public void Unlock()
        {
            if (!drawBuffer.IsLock) return;
            drawBuffer.Unlock();
        }

        public void DrawMesh(in Transform transform, in Mesh mesh, in Material material)
        {
            if (!DrawCheck(transform)) return;
            RenderTri(mesh.Vertices, transform, material);
            // DrawAxes(transform);
        }

        public void DrawGrid()
        {
            var tr = new Transform();
            tr.Update();
            RenderLine(Gizmos.Grids.ToList(), tr);
        }

        private void DrawAxes(in Transform transform)
        {
            RenderLine(Gizmos.Axes.ToList(), transform);
        }

        public bool DrawCheck(Transform transform)
        {
            var viewPos = Vector4.Transform(new Vector4(transform.Position, 1), vMat);
            return frustum.Check(viewPos.ToVector3()) != Frustum.Result.Outside;
        }

        private bool BackfaceCulling(List<OutputVS> vertices)
        {
            var edge1 = vertices[1].Vector3 - vertices[0].Vector3;
            var edge2 = vertices[2].Vector3 - vertices[0].Vector3;
            // cw backface culling
            var faceNormal = -Vector3.Cross(edge1, edge2);
            if (Vector3.Dot(faceNormal, Vector3.UnitZ) >= 0f)
                return false;
            return true;
        }

        private void RenderTri(in List<Vertex> vertices, in Transform transform, in Material material)
        {
            if (drawBuffer.IsLock)
            {
                List<OutputVS> vs = new List<OutputVS>();
                foreach (var v in vertices) 
                    vs.Add(material.Convert(v, transform.Mat, pvMat));

                var triCount = vs.Count / 3;
                for (int i = 0; i < triCount; i++)
                {
                    var tri = vs.GetRange(i * 3, 3);
                    RenderTriOnce(tri, material);
                }
            }
        }

        private void RenderTriOnce(List<OutputVS> vertices, in Material material)
        {
            ClipTriangles.Clip(ref vertices);
            // View to NDC
            for(int i = 0; i < vertices.Count; i++)
                vertices[i] = OutputVS.ToNDC(vertices[i]);
            var triCount = vertices.Count / 3;
            for (int j = 0; j < triCount; j++)
            {
                var tri = vertices.GetRange(j * 3, 3);
                if (!BackfaceCulling(tri)) continue;
                for (int i = 0; i < tri.Count; i++)
                    tri[i] = OutputVS.ToScreen(tri[i], Screen);
                drawBuffer.DrawTriangle(Screen, tri[0], tri[1], tri[2], material);
                // drawBuffer.DrawWireframe(Screen, tri[0], tri[1], tri[2]);
            }
        }

        public OutputVS LineShader(InputVS input)
        {
            return new OutputVS()
            {
                Position = Vector4.Transform(input.Position, input.TMat * input.PVMat),
                Normal = Vector3.TransformNormal(input.Normal, input.TMat),
                UVs = input.UVs.ToArray(),
                Color = input.Color,
            };
        }

        private void RenderLine(List<Vertex> vertices, in Transform transform)
        {
            List<OutputVS> vs = new List<OutputVS>();
            foreach (var v in vertices)
                vs.Add(LineShader(new InputVS {
                    Position = v.Vector4,
                    Normal = v.Normal,
                    UVs = new Vector2[] { v.UV, v.UV },
                    Color = v.Color,
                    TMat = transform.Mat,
                    PVMat = pvMat,
                }));

            var lineCount = vertices.Count / 2;
            for (int i = 0; i < lineCount; i++)
            {
                var line = vs.GetRange(i * 2, 2);
                RenderLineOnce(line);
            }
        }

        private void RenderLineOnce(List<OutputVS> vertices)
        {
            ClipLines.Clip(ref vertices);
            // View to NDC
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = OutputVS.ToNDC(vertices[i]);
            var lineCount = vertices.Count / 2;
            for (int j = 0; j < lineCount; j++)
            {
                var line = vertices.GetRange(j * 2, 2);
                for (int i = 0; i < line.Count; i++)
                    line[i] = OutputVS.ToScreen(line[i], Screen);
                drawBuffer.DrawLine(Screen, line[0], line[1]);
            }
        }
    }
}
