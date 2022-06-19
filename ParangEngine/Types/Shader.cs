using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public struct InputVS
    {
        public Vector4 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2[] UVs { get; set; }
        public Color Color { get; set; }
        public Matrix4x4 TMat { get; set; }
        public Matrix4x4 PVMat { get; set; }
    }

    public struct OutputVS
    {
        public Vector4 Position
        {
            get => position;
            set
            {
                position = value;
                Vector3 = position.ToVector3();
                Vector2 = position.ToVector2();
            }
        }
        public Vector3 Vector3 { get; private set; }
        public Vector2 Vector2 { get; private set; }
        public Vector3 Normal
        {
            get => normal;
            set
            {
                normal = Vector3.Normalize(value);
                var c = Vector3.Cross(Vector3.UnitZ, normal);
                var s = (float)Math.Sqrt((1f + Vector3.Dot(Vector3.UnitZ, normal)) * 2f);
                RotNormal = MathExtension.RotYPI;
                if (s != 0) RotNormal = new Quaternion(c.X / s, c.Y / s, c.Z / s, s / 2f);
            }
        }
        public Quaternion RotNormal { get; set; }
        public Vector2[] UVs { get; set; }
        public Color Color { get; set; }
        public Vector4 View { get; set; }

        public float X => position.X;
        public float Y => position.Y;
        public float Z => position.Z;
        public float W => position.W;

        private Vector4 position;
        private Vector3 normal;

        static public OutputVS ToNDC(OutputVS v)
        {
            v.Position = v.View = v.Position.ToNDC();
            return v;
        }

        static public OutputVS ToScreen(OutputVS v, Screen screen)
        {
            v.Position = v.Position.ToScreen(screen);
            return v;
        }

        static public OutputVS Blend(OutputVS v1, OutputVS v2, float t)
        {
            t = t < 0 || 1 < t ? 1 : t;
            var uvs = new List<Vector2>();
            int count = Math.Min(v1.UVs.Length, v2.UVs.Length);
            for(int i = 0; i < count; i++)
                uvs.Add(v1.UVs[i] * (1f - t) + v2.UVs[i] * t);
            return new OutputVS
            {
                Position = v1.Position * (1f - t) + v2.Position * t,
                Normal = Vector3.Normalize(v1.Normal + v2.Normal),
                UVs = uvs.ToArray(),
                Color = v1.Color * (1f - t) + v2.Color * t,
            };
        }
    }

    public struct InputPS
    {
        public Color Color { get; set; }
        public Texture[] Textures { get; set; }
        public Vector2[] UVs { get; set; }
        public Vector3 Normal { get; set; }
        public Quaternion RotNormal { get; set; }
        public Color VertexColor { get; set; }

        public Color GetSample(Material.Type type)
        {
            int index = (int)type;
            return Textures[index]?.GetSample(UVs[index % UVs.Length]) ?? Color.White;
        }
    }

    public struct OutputPS
    {
        public Color Color { get; set; }
        public Vector3 Normal { get; set; }
    }
}
