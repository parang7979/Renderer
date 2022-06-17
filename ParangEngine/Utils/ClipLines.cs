using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    static public class ClipLines
    {
        static public void Clip(ref List<OutputVS> vertices)
        {
            foreach (var c in Clipper.Clippers) ClipOnce(ref vertices, c);
        }

        static private int GetInsideIndex(in List<bool> results)
        {
            if (results[0]) return 1;
            return 0;
        }

        static private void ClipOnce(ref List<OutputVS> vertices, (Func<OutputVS, bool>, Func<OutputVS, OutputVS, OutputVS>) clipper)
        {
            List<bool> results = new List<bool>();
            int lines = vertices.Count / 2;
            for (int i = 0; i < lines; i++)
            {
                var index = i * 2;
                var nonPass = 0;
                for (int j = 0; j < 2; j++)
                {
                    var res = clipper.Item1(vertices[index + j]);
                    results.Add(res);
                    if (res) nonPass++;
                }
                if (nonPass == 0) continue;
                else if (nonPass == 1)
                    Clip(ref vertices, index, GetInsideIndex(results), clipper.Item2);
                else
                {
                    vertices.RemoveRange(i, 2);
                    lines--;
                    i--;
                }
            }
        }

        static private void Clip(ref List<OutputVS> vertices, int index, int inSide, Func<OutputVS, OutputVS, OutputVS> clip)
        {
            int outside = index + ((inSide + 1) % 2);
            var outV = vertices[outside];
            var inV = vertices[index + inSide];
            var clipV = clip(inV, outV);
            vertices[outside] = clipV;
        }
    }
}
