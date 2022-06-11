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
    public class Mesh
    {
        public int Id { get; set; }

        // private List<Vertex> vertices;
        // private List<int> indicies;

        private List<Vertex> mesh;

        public Mesh(int id, List<Vertex> vertices, List<int> indicies)
        {
            Id = id;
            // this.vertices = vertices;
            // this.indicies = indicies;
            Dictionary<int, Vector3> normals = new Dictionary<int, Vector3>();
            var triCount = indicies.Count / 3;
            for (int i = 0; i < triCount; i++)
            {
                var v1 = vertices[indicies[i * 3]];
                var v2 = vertices[indicies[i * 3 + 1]];
                var v3 = vertices[indicies[i * 3 + 2]];
                var edge1 = v2.Vector3 - v1.Vector3;
                var edge2 = v3.Vector3 - v1.Vector3;
                var faceNormal = Vector3.Normalize(Vector3.Cross(edge1, edge2));
                for (int j = 0; j < 3; j++)
                {
                    int k = indicies[i * 3 + j];
                    if (normals.ContainsKey(k)) normals[k] = (normals[k] + faceNormal) / 2;
                    else normals.Add(k, faceNormal);
                }
            }
            foreach (var n in normals)
            {
                var v = vertices[n.Key];
                {
                    v.SetNormal(n.Value);
                    vertices[n.Key] = v;
                }
                mesh = indicies.Select(x => vertices[x]).ToList();
            }
        }

        public void Render(in Camera camera, in Transform transform, in Texture texture)
        {
            // 애니메이션, 스키닝, 등등..
            camera.RenderTri(mesh, transform, texture,
                (v, m) => Vertex.Transform(v, m));
            camera.DrawAxes(transform);
        }
    }
}
