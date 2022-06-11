using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    static public class Clipper
    {
        static public List<(Func<Vertex, bool>, Func<Vertex, Vertex, Vertex>)> Clippers =
            new List<(Func<Vertex, bool>, Func<Vertex, Vertex, Vertex>)>
            {
                { (TestW0, ClipW0) },
                { (TestNY, ClipNY) },
                { (TestPY, ClipPY) },
                { (TestNX, ClipNX) },
                { (TestPX, ClipPX) },
                { (TestPZ, ClipPZ) },
                { (TestNZ, ClipNZ) },
            };

        static bool TestW0(Vertex v) => v.W < 0f;
        static Vertex ClipW0(Vertex v1, Vertex v2)
        {
            var p1 = v1.W;
            var p2 = v2.W;
            var t = p1 / (p1 - p2);
            return v1 * (1f - t) + v2 * t;
        }

        static bool TestNY(Vertex v) => v.Y < -v.W;
        static Vertex ClipNY(Vertex v1, Vertex v2)
        {
            var p1 = v1.W + v1.Y;
            var p2 = v2.W + v2.Y;
            var t = p1 / (p1 - p2);
            return v1 * (1f - t) + v2 * t;
        }
        static bool TestPY(Vertex v) => v.Y > v.W;
        static Vertex ClipPY(Vertex v1, Vertex v2)
        {
            var p1 = v1.W - v1.Y;
            var p2 = v2.W - v2.Y;
            var t = p1 / (p1 - p2);
            return v1 * (1f - t) + v2 * t;
        }

        static bool TestNX(Vertex v) => v.X < -v.W;
        static Vertex ClipNX(Vertex v1, Vertex v2)
        {
            var p1 = v1.W + v1.X;
            var p2 = v2.W + v2.X;
            var t = p1 / (p1 - p2);
            return v1 * (1f - t) + v2 * t;
        }
        static bool TestPX(Vertex v) => v.X > v.W;
        static Vertex ClipPX(Vertex v1, Vertex v2)
        {
            var p1 = v1.W - v1.X;
            var p2 = v2.W - v2.X;
            var t = p1 / (p1 - p2);
            return v1 * (1f - t) + v2 * t;
        }

        static bool TestPZ(Vertex v) => v.Z > v.W;
        static Vertex ClipPZ(Vertex v1, Vertex v2)
        {
            var p1 = v1.W - v1.Z;
            var p2 = v2.W - v2.Z;
            var t = p1 / (p1 - p2);
            return v1 * (1f - t) + v2 * t;
        }
        static bool TestNZ(Vertex v) => v.Z < -v.W;
        static Vertex ClipNZ(Vertex v1, Vertex v2)
        {
            var p1 = v1.W + v1.Z;
            var p2 = v2.W + v2.Z;
            var t = p1 / (p1 - p2);
            return v1 * (1f - t) + v2 * t;
        }
    }

    static public class ClipTriangles
    {
        static public void Clip(ref List<Vertex> vertices)
        {
            foreach (var c in Clipper.Clippers) ClipOnce(ref vertices, c);
        }

        static private int GetOutsideIndex(in List<bool> results)
        {
            if (!results[0])
            {
                return results[1] ? 1 : 2;
            }
            return 0;
        }

        static private int GetInsideIndex(in List<bool> results)
        {
            if (results[0])
            {
                return !results[1] ? 1 : 2;
            }
            return 0;
        }

        static private void ClipOnce(ref List<Vertex> vertices, (Func<Vertex, bool>, Func<Vertex, Vertex, Vertex>)clipper)
        {
            List<bool> results = new List<bool>();
            int tris = vertices.Count / 3;
            for (int i = 0; i < tris; i++)
            {
                var index = i * 3;
                var nonPass = 0;
                for (int j = 0; j < 3; j++)
                {
                    var res = clipper.Item1(vertices[index + j]);
                    results.Add(res);
                    if (res) nonPass++;
                }
                if (nonPass == 0) continue;
                else if (nonPass == 1)
                    DivideTwo(ref vertices, index, GetOutsideIndex(results), clipper.Item2);
                else if (nonPass == 2)
                    Clip(ref vertices, index, GetInsideIndex(results), clipper.Item2);
                else
                {
                    vertices.RemoveRange(i, 3);
                    tris--;
                    i--;
                }
            }
        }

        static private void Clip(ref List<Vertex> vertices, int index, int inSide, Func<Vertex, Vertex, Vertex> clip)
        {
            int outside1 = index + ((inSide + 1) % 3);
            int outside2 = index + ((inSide + 2) % 3);
            var outV1 = vertices[outside1];
            var outV2 = vertices[outside2];
            var inV = vertices[index + inSide];
            var clipV1 = clip(inV, outV1);
            var clipV2 = clip(inV, outV2);
            vertices[outside1] = clipV1;
            vertices[outside2] = clipV2;
        }

        static private void DivideTwo(ref List<Vertex> vertices, int index, int outSide, Func<Vertex, Vertex, Vertex> clip)
        {
            int inside1 = index + ((outSide + 1) % 3);
            int inside2 = index + ((outSide + 2) % 3);
            var inV1 = vertices[inside1];
            var inV2 = vertices[inside2];
            var outV = vertices[index + outSide];

            var clipV1 = clip(outV, inV1);
            var clipV2 = clip(outV, inV2);

            vertices[index] = clipV1;
            vertices[index + 1] = inV1;
            vertices[index + 2] = inV2;
            vertices.Add(clipV1);
            vertices.Add(inV2);
            vertices.Add(clipV2);
        }
    }
}
