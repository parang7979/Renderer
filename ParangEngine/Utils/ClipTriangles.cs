using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    static public class ClipTriangles
    {
        static private List<(Func<Vertex, bool>, Func<Vertex, Vertex, Vertex>)> clippers =
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

        static bool TestW0(Vertex v) => v.Pos.W < 0f;
        static Vertex ClipW0(Vertex v1, Vertex v2)
        {
            var p1 = v1.Pos.W;
            var p2 = v2.Pos.W;
            var t = p1 / (p1 - p2);
            return new Vertex(v1.Pos * (1f - t) + v2.Pos * t);
        }

        static bool TestNY(Vertex v) => v.Pos.Y < -v.Pos.W;
        static Vertex ClipNY(Vertex v1, Vertex v2)
        {
            var p1 = v1.Pos.W + v1.Pos.Y;
            var p2 = v2.Pos.W + v2.Pos.Y;
            var t = p1 / (p1 - p2);
            return new Vertex(v1.Pos * (1f - t) + v2.Pos * t);
        }
        static bool TestPY(Vertex v) => v.Pos.Y < v.Pos.W;
        static Vertex ClipPY(Vertex v1, Vertex v2)
        {
            var p1 = v1.Pos.W - v1.Pos.Y;
            var p2 = v2.Pos.W - v2.Pos.Y;
            var t = p1 / (p1 - p2);
            return new Vertex(v1.Pos * (1f - t) + v2.Pos * t);
        }

        static bool TestNX(Vertex v) => v.Pos.X < -v.Pos.W;
        static Vertex ClipNX(Vertex v1, Vertex v2)
        {
            var p1 = v1.Pos.W + v1.Pos.X;
            var p2 = v2.Pos.W + v2.Pos.X;
            var t = p1 / (p1 - p2);
            return new Vertex(v1.Pos * (1f - t) + v2.Pos * t);
        }
        static bool TestPX(Vertex v) => v.Pos.X < v.Pos.W;
        static Vertex ClipPX(Vertex v1, Vertex v2)
        {
            var p1 = v1.Pos.W - v1.Pos.X;
            var p2 = v2.Pos.W - v2.Pos.X;
            var t = p1 / (p1 - p2);
            return new Vertex(v1.Pos * (1f - t) + v2.Pos * t);
        }

        static bool TestPZ(Vertex v) => v.Pos.Z < v.Pos.W;
        static Vertex ClipPZ(Vertex v1, Vertex v2)
        {
            var p1 = v1.Pos.W - v1.Pos.Z;
            var p2 = v2.Pos.W - v2.Pos.Z;
            var t = p1 / (p1 - p2);
            return new Vertex(v1.Pos * (1f - t) + v2.Pos * t);
        }
        static bool TestNZ(Vertex v) => v.Pos.Z < -v.Pos.W;
        static Vertex ClipNZ(Vertex v1, Vertex v2)
        {
            var p1 = v1.Pos.W + v1.Pos.Z;
            var p2 = v2.Pos.W + v2.Pos.Z;
            var t = p1 / (p1 - p2);
            return new Vertex(v1.Pos * (1f - t) + v2.Pos * t);
        }
        
        static public void ClipTriangle(List<Vertex> vertices)
        {
            List<bool> test = new List<bool>();
            for (int i = 0; i < vertices.Count / 3; i++)
            {
                var index = i * 3;
                var nonPass = 0;
                for (int j = 0; j < 3; j++)
                {
                    var res = clippers[i].Item1(vertices[i + j]);
                    test.Add(res);
                    if (res) nonPass++;
                }
            }
        }
    }
}
