using ParangEngine.Utils;
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
        public float X { get => pos.X; set => pos.X = value; }
        public float Y { get => pos.Y; set => pos.Y = value; }
        public float Z { get => pos.Z; set => pos.Z = value; }
        public float W { get => pos.W; set => pos.W = value; }
        public Vector4 Vector4 => pos;
        public Vector3 Vector3 => pos.ToVector3();
        public Vector2 Vector2 => pos.ToVector2();
        public Vector2 UV => uv;
        public Color Color => color;

        private Vector4 pos;
        private Vector2 uv;
        private Color color;

        public Vertex(in Vector4 v)
        {
            pos = v;
            uv = Vector2.Zero;
            this.color = Color.White;
        }

        public Vertex(in Vector4 v, in Color color)
        {
            pos = v;
            uv = Vector2.Zero;
            this.color = color;
        }

        public Vertex(in Vector4 v, in Vector2 uv)
        {
            pos = v;
            this.uv = uv;
            this.color = Color.White;
        }

        public Vertex(in Vector4 v, in Vector2 uv, in Color color)
        {
            pos = v;
            this.uv = uv;
            this.color = color;
        }

        public Vertex(in Vector4 v, in Vector2 uv, string color)
        {
            pos = v;
            this.uv = uv;
            this.color = new Color(color);
        }

        static public Vertex operator* (Vertex v, float t)
        {
            return new Vertex(v.pos * t, v.UV * t, v.Color * t);
        }

        static public Vertex operator+ (Vertex v1, Vertex v2)
        {
            return new Vertex(v1.pos + v2.pos, v1.UV + v2.UV, v1.Color + v2.Color);
        }

        static public Vertex operator- (Vertex v1, Vertex v2)
        {
            return new Vertex(v1.pos - v2.pos, v1.UV - v2.UV, v1.Color - v2.Color);
        }

        public float Dot(in Vertex v)
        {
            return Vector4.Dot(pos, v.pos);
        }        

        static public Vertex Transform(Vertex v, Matrix4x4 mat)
        {
            v.pos = Vector4.Transform(v.pos, mat);
            return v;
        }
    }
}
