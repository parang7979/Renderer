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
        static public List<(Func<OutputVS, bool>, Func<OutputVS, OutputVS, OutputVS>)> Clippers =
            new List<(Func<OutputVS, bool>, Func<OutputVS, OutputVS, OutputVS>)>
            {
                { (TestW0, ClipW0) },
                { (TestNY, ClipNY) },
                { (TestPY, ClipPY) },
                { (TestNX, ClipNX) },
                { (TestPX, ClipPX) },
                { (TestPZ, ClipPZ) },
                { (TestNZ, ClipNZ) },
            };

        static bool TestW0(OutputVS v) => v.W < 0f;
        static OutputVS ClipW0(OutputVS v1, OutputVS v2)
        {
            var p1 = v1.W;
            var p2 = v2.W;
            var t = p1 / (p1 - p2);
            return OutputVS.Blend(v1, v2, t);
        }

        static bool TestNY(OutputVS v) => v.Y < -v.W;
        static OutputVS ClipNY(OutputVS v1, OutputVS v2)
        {
            var p1 = v1.W + v1.Y;
            var p2 = v2.W + v2.Y;
            var t = p1 / (p1 - p2);
            return OutputVS.Blend(v1, v2, t);
        }
        static bool TestPY(OutputVS v) => v.Y > v.W;
        static OutputVS ClipPY(OutputVS v1, OutputVS v2)
        {
            var p1 = v1.W - v1.Y;
            var p2 = v2.W - v2.Y;
            var t = p1 / (p1 - p2);
            return OutputVS.Blend(v1, v2, t);
        }

        static bool TestNX(OutputVS v) => v.X < -v.W;
        static OutputVS ClipNX(OutputVS v1, OutputVS v2)
        {
            var p1 = v1.W + v1.X;
            var p2 = v2.W + v2.X;
            var t = p1 / (p1 - p2);
            return OutputVS.Blend(v1, v2, t);
        }
        static bool TestPX(OutputVS v) => v.X > v.W;
        static OutputVS ClipPX(OutputVS v1, OutputVS v2)
        {
            var p1 = v1.W - v1.X;
            var p2 = v2.W - v2.X;
            var t = p1 / (p1 - p2);
            return OutputVS.Blend(v1, v2, t);
        }

        static bool TestPZ(OutputVS v) => v.Z > v.W;
        static OutputVS ClipPZ(OutputVS v1, OutputVS v2)
        {
            var p1 = v1.W - v1.Z;
            var p2 = v2.W - v2.Z;
            var t = p1 / (p1 - p2);
            return OutputVS.Blend(v1, v2, t);
        }
        static bool TestNZ(OutputVS v) => v.Z < -v.W;
        static OutputVS ClipNZ(OutputVS v1, OutputVS v2)
        {
            var p1 = v1.W + v1.Z;
            var p2 = v2.W + v2.Z;
            var t = p1 / (p1 - p2);
            return OutputVS.Blend(v1, v2, t);
        }
    }

    static public class ClipTriangles
    {
        static public void Clip(ref List<OutputVS> vertices)
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

        static private void ClipOnce(ref List<OutputVS> vertices, (Func<OutputVS, bool>, Func<OutputVS, OutputVS, OutputVS>)clipper)
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

        static private void Clip(ref List<OutputVS> vertices, int index, int inSide, Func<OutputVS, OutputVS, OutputVS> clip)
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

        static private void DivideTwo(ref List<OutputVS> vertices, int index, int outSide, Func<OutputVS, OutputVS, OutputVS> clip)
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
