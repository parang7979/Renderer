using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    static public class Gizmos
    {
        static public List<(Vector4, Vector4)> Grids { get; private set; }

        static public void CreateGrid(int size)
        {
            Grids = new List<(Vector4, Vector4)>();
            for (int x = -size; x < size; x++)
                Grids.Add((new Vector4(x, 0, -size, 1),
                    new Vector4(x, 0, size, 1)));

            for (int y = -size; y < size; y++)
                Grids.Add((new Vector4(-size, 0, y, 1),
                    new Vector4(size, 0, y, 1)));
        }
    }
}
