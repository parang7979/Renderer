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
        public Vector3 Normal => normal;
        public Color Color => color;

        private Vector4 pos;
        private Vector3 normal;
        private Vector2 uv;
        private Color color;

        public Vertex(Vector3 v, float w, string color)
        {
            pos = new Vector4(v, w);
            normal = Vector3.Zero;
            uv = Vector2.Zero;
            this.color = new Color(color);
        }

        public Vertex(float x, float y, float z, float w, string color)
        {
            pos = new Vector4(x, y, z, w);
            normal = Vector3.Zero;
            uv = Vector2.Zero;
            this.color = new Color(color);
        }

        public Vertex(Vector4 v, Vector3 n = default, Vector2 uv = default, Color color = default)
        {
            pos = v;
            normal = n;
            this.uv = uv;
            this.color = color;
        }

        static public Vertex operator* (Vertex v, float t)
        {
            return new Vertex(v.pos * t, v.normal, v.UV * t, v.Color * t);
        }

        static public Vertex operator+ (Vertex v1, Vertex v2)
        {
            return new Vertex(v1.pos + v2.pos, Vector3.Normalize(v1.Normal + v2.Normal), v1.UV + v2.UV, v1.Color + v2.Color);
        }

        static public Vertex operator- (Vertex v1, Vertex v2)
        {
            return new Vertex(v1.pos - v2.pos, Vector3.Normalize(v1.Normal - v2.Normal), v1.UV - v2.UV, v1.Color - v2.Color);
        }

        static public float Dot(in Vertex v1, in Vertex v2)
        {
            return Vector4.Dot(v1.pos, v2.pos);
        }        

        static public Vertex UpdateNormal(Vertex v, Vector3 n)
        {
            v.normal = Vector3.Normalize(n);
            return v;
        }

        static public Vertex Transform(Vertex v, Matrix4x4 mat)
        {
            v.pos = Vector4.Transform(v.pos, mat);
            v.normal = Vector4.Transform(new Vector4(v.normal, 0), mat).ToVector3();
            return v;
        }
    }
}
