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

        static private void ParseIndex(string[] strs, ref List<int> fv, ref List<int> ft, ref List<int> fn)
        {
            List<int> v = new List<int>();
            List<int> t = new List<int>();
            List<int> n = new List<int>();
            for (int i = 1; i < strs.Length; i++)
            {
                var fstrs = strs[i].Split('/');
                v.Add(fstrs[0].SafeParse(0));
                if (fstrs.Length > 1)
                    t.Add(fstrs[1].SafeParse(0));
                if (fstrs.Length > 2)
                    n.Add(fstrs[2].SafeParse(0));
            }
            for (int i = 1; i < v.Count - 1; i++)
            {
                fv.Add(v[0] - 1);
                if (0 < t.Count) ft.Add(t[0] - 1);
                if (1 < n.Count) fn.Add(n[0] - 1);
                fv.Add(v[i] - 1);
                if (i < t.Count) ft.Add(t[i] - 1);
                if (i < n.Count) fn.Add(n[i] - 1);
                fv.Add(v[i + 1] - 1);
                if (i + 1 < t.Count) ft.Add(t[i + 1] - 1);
                if (i + 1 < n.Count) fn.Add(n[i + 1] - 1);
            }
            fv.RemoveAll(x => x < 0);
            ft.RemoveAll(x => x < 0);
            fn.RemoveAll(x => x < 0);
        }

        static public Dictionary<string, Mesh> LoadMesh(string path)
        {
            List<Vector3> v = new List<Vector3>();
            List<Vector2> t = new List<Vector2>();
            List<Vector3> n = new List<Vector3>();
            List<int> fv = new List<int>();
            List<int> ft = new List<int>();
            List<int> fn = new List<int>();
            Dictionary<string, Mesh> ret = new Dictionary<string, Mesh>();
            using (var sr = File.OpenText(path))
            {
                string name = path;
                while (!sr.EndOfStream)
                {
                    var strs = sr.ReadLine().Split(' ');
                    switch (strs[0])
                    {
                        case "v":
                            v.Add(strs.ToVector3(1));
                            break;

                        case "vt":
                            t.Add(strs.ToVector2(1));
                            break;

                        case "vn":
                            n.Add(strs.ToVector3(1));
                            break;

                        case "f":
                            ParseIndex(strs, ref fv, ref ft, ref fn);
                            break;

                        case "o":
                            if (v.Count > 0 && fv.Count > 0)
                                ret.Add(name, new Mesh(v, t, n, fv, ft, fn));
                            name = strs[1];
                            fv.Clear();
                            ft.Clear();
                            fn.Clear();
                            break;

                        case "g":
                            if (v.Count > 0 && fv.Count > 0)
                                ret.Add(name, new Mesh(v, t, n, fv, ft, fn));
                            name = strs[1];
                            fv.Clear();
                            ft.Clear();
                            fn.Clear();
                            break;
                    }
                }
                if (v.Count > 0 && fv.Count > 0)
                    ret.Add(name, new Mesh(v, t, n, fv, ft, fn));
            }
            return ret;
        }

        public Mesh(List<Vector3> v, List<Vector2> t, List<Vector3> n, List<int> vi, List<int> ti, List<int> ni)
        {
            BuildMesh(v, t, n, vi, ti, ni);
        }

        private List<Vector3> CreateNormal(List<Vector3> v, List<int> vi)
        {
            Vector3[] normals = new Vector3[v.Count];
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
                    normals[k] = (normals[k] + faceNormal) / 2;
                }
            }
            return normals.ToList();
        }

        private void BuildMesh(List<Vector3> v, List<Vector2> t, List<Vector3> n, List<int> vi, List<int> ti, List<int> ni)
        {
            if (ni == null || ni.Count == 0) n = CreateNormal(v, vi);
            vertices = new List<Vertex>();
            for (int i = 0; i < vi.Count; i++)
            {
                int x = vi[i];
                int y = ti != null && ti.Count == vi.Count ? ti[i] : x;
                int z = ni != null && ni.Count == vi.Count ? ni[i] : x;
                vertices.Add(new Vertex(v[x], y < t.Count ? t[y] : Vector2.Zero, z < n.Count ? n[z] : Vector3.Zero));
            }
        }
    }
}
