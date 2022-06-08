using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public struct Vertex
    {
        public Vector4 Pos { get; set; }
        public Vector2 UV { get; set; }
        public Color Color { get; set; }

        public Vertex(in Vector4 v, in Vector2 uv)
        {
            Pos = v;
            UV = uv;
            Color = new Color(1f, 1f, 1f);
        }

        public Vertex(in Vector4 v, in Vector2 uv, in Color color)
        {
            Pos = v;
            UV = uv;
            Color = color;
        }

        public Vertex(in Vector4 v, in Vector2 uv, string color)
        {
            Pos = v;
            UV = uv;
            Color = new Color(color);
        }

        public float X => Pos.X;
        public float Y => Pos.Y;
        public float Z => Pos.Z;
        public float W => Pos.W;
    }
}
