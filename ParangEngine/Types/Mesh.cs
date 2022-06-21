using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class Mesh
    {
        public List<Vertex> Vertices => vertices.ToList();

        private List<Vertex> vertices;

        public Mesh(string path)
        {
            List<Vector3> v = new List<Vector3>();
            List<Vector2> t = new List<Vector2>();
            List<int> fv = new List<int>();
            List<int> ft = new List<int>();

            using(var sr = File.OpenText(path))
            {
                while(!sr.EndOfStream)
                {
                    var strs = sr.ReadLine().Split(' ');
                    switch(strs[0])
                    {
                        case "v":
                            v.Add(strs.ToVector3(1));
                            break;

                        case "vt":
                            t.Add(strs.ToVector2(1));
                            break;

                        case "f":
                            ParseIndex(strs, ref fv, ref ft);
                            break;
                    }
                }
            }
            BuildMesh(v, t, fv, ft);
        }

        public Mesh(List<Vector3> v, List<Vector2> t, List<int> vi, List<int> ti = null)
        {
            BuildMesh(v, t, vi, ti);
        }

        private void ParseIndex(string[] strs, ref List<int> fv, ref List<int> ft)
        {
            List<int> v = new List<int>();
            List<int> t = new List<int>();
            for (int i = 1; i < strs.Length; i++)
            {
                var fstrs = strs[i].Split('/');
                v.Add(fstrs[0].SafeParse(0));
                if (fstrs.Length > 2)
                    t.Add(fstrs[1].SafeParse(0));
            }
            for (int i = 1; i < v.Count - 1; i++)
            {
                fv.Add(v[0] - 1);
                if (0 < t.Count) ft.Add(t[0] - 1);
                fv.Add(v[i] - 1);
                if (i < t.Count) ft.Add(t[i] - 1);
                fv.Add(v[i + 1] - 1);
                if (i + 1 < t.Count) ft.Add(t[i + 1] - 1);
            }
        }

        private void BuildMesh(List<Vector3> v, List<Vector2> t, List<int> vi, List<int> ti = null)
        {
            Dictionary<int, Vector3> normals = new Dictionary<int, Vector3>();
            var triCount = vi.Count / 3;
            for (int i = 0; i < triCount; i++)
            {
                var v1 = v[vi[i * 3]];
                var v2 = v[vi[i * 3 + 1]];
                var v3 = v[vi[i * 3 + 2]];
                var edge1 = v2 - v1;
                var edge2 = v3 - v1;
                var faceNormal = Vector3.Normalize(Vector3.Cross(edge1, edge2));
                for (int j = 0; j < 3; j++)
                {
                    int k = vi[i * 3 + j];
                    if (normals.ContainsKey(k)) normals[k] = (normals[k] + faceNormal) / 2;
                    else normals.Add(k, faceNormal);
                }
            }
            vertices = new List<Vertex>();
            for (int i = 0; i < vi.Count; i++)
            {
                int x = vi[i];
                int y = ti != null && ti.Count == vi.Count ? ti[i] : x;
                vertices.Add(new Vertex(v[x], normals[x], y < t.Count ? t[y] : Vector2.Zero));
            }
        }
    }
}
