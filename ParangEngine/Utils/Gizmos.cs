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
        static public readonly List<Vertex> Axes = new List<Vertex>{
            new Vertex(Vector3.Zero, "red"),
            new Vertex(Vector3.UnitX, "red"),
            new Vertex(Vector3.Zero, "green"),
            new Vertex(Vector3.UnitY, "green"),
            new Vertex(Vector3.Zero, "blue"),
            new Vertex(Vector3.UnitZ, "blue"),
        };

        static public List<Vertex> Grids { get; private set; }

        static public void CreateGrid(int size)
        {
            Grids = new List<Vertex>();
            for (int x = -size; x < size; x++)
            {
                Grids.Add(new Vertex(x, 0, -size, "white"));
                Grids.Add(new Vertex(x, 0, size, "white"));
            }

            for (int y = -size; y < size; y++)
            {
                Grids.Add(new Vertex(-size, 0, y, "white"));
                Grids.Add(new Vertex(size, 0, y, "white"));
            }
        }
    }
}
