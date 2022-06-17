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
        
        public bool DrawCheck(Transform transform)
        {
            var viewPos = Vector4.Transform(new Vector4(transform.Position, 1), vMat);
            return frustum.Check(viewPos.ToVector3()) != Frustum.Result.Outside;
        }

        public void DrawMesh(in Transform transform, in Mesh mesh, in Material material)
        {
            if (!DrawCheck(transform)) return;
            RenderTri(mesh.Vertices, transform, material);
            DrawAxes(transform);
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

        private void RenderTri(in List<Vertex> vertices, in Transform transform, in Material material)
        {
            if (gBuffer.IsLock)
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
            foreach (var v in vertices) v.ToNDC();
            var triCount = vertices.Count / 3;
            for (int j = 0; j < triCount; j++)
            {
                var tri = vertices.GetRange(j * 3, 3);
                if (!BackfaceCulling(tri)) continue;
                foreach (var t in tri) t.ToScreen(Screen);
                gBuffer.DrawTriangle(Screen, tri[0], tri[1], tri[2], material);
                gBuffer.DrawWireframe(Screen, tri[0], tri[1], tri[2]);
            }
        }

        private void RenderLine(List<Vertex> vertices, in Transform transform)
        {
            List<OutputVS> vs = new List<OutputVS>();
            foreach (var v in vertices)
                vs.Add(Shaders.DefaultVS(new InputVS {
                    Position = v.Vector4,
                    Normal = v.Normal,
                    UVs = new Vector2[] { v.UV, v.UV },
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
            foreach (var v in vertices) v.ToNDC();
            var lineCount = vertices.Count / 2;
            for (int j = 0; j < lineCount; j++)
            {
                var line = vertices.GetRange(j * 2, 2);
                foreach (var l in line) l.ToScreen(Screen);
                gBuffer.DrawLine(Screen, line[0], line[1]);
            }
        }
    }
}
