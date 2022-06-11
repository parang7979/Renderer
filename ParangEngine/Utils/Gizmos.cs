using ParangEngine.Types;
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
        static public List<Vertex> Grids { get; private set; }

        static public void CreateGrid(int size)
        {
            Grids = new List<Vertex>();
            for (int x = -size; x < size; x++)
            {
                Grids.Add(new Vertex(x, 0, -size, 1, "white"));
                Grids.Add(new Vertex(x, 0, size, 1, "white"));
            }

            for (int y = -size; y < size; y++)
            {
                Grids.Add(new Vertex(-size, 0, y, 1, "white"));
                Grids.Add(new Vertex(size, 0, y, 1, "white"));
            }
        }
    }
}
