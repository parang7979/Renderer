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
        public Vector4 View => view;

        private Vector4 pos;
        private Vector3 normal;
        private Vector2 uv;
        private Color color;
        private Vector4 view;

        public Vertex(Vector3 v)
        {
            pos = new Vector4(v, 1);
            normal = Vector3.Zero;
            uv = Vector2.Zero;
            color = Color.White;
            view = Vector4.Zero;
        }

        public Vertex(Vector3 v, Vector3 n, Vector2 uv)
        {
            pos = new Vector4(v, 1);
            normal = n;
            this.uv = uv;
            color = Color.White;
            view = Vector4.Zero;
        }

        public Vertex(Vector3 v, string color)
        {
            pos = new Vector4(v, 1);
            normal = Vector3.Zero;
            uv = Vector2.Zero;
            this.color = new Color(color);
            view = Vector4.Zero;
        }

        public Vertex(float x, float y, float z, string color)
        {
            pos = new Vector4(x, y, z, 1);
            normal = Vector3.Zero;
            uv = Vector2.Zero;
            this.color = new Color(color);
            view = Vector4.Zero;
        }
    }
}
