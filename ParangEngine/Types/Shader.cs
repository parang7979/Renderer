using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class InputVS
    {
        public Vector4 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2[] UVs { get; set; }
        public Color Color { get; set; }
        public Matrix4x4 TMat { get; set; }
        public Matrix4x4 PVMat { get; set; }
    }

    public class OutputVS
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
        public Vector3 Normal { get; set; }
        public Vector2[] UVs { get; set; }
        public Color Color { get; set; }
        public Vector4 View { get; set; }

        public float X => position.X;
        public float Y => position.Y;
        public float Z => position.Z;
        public float W => position.W;

        private Vector4 position;

        public void ToNDC()
        {
            Position = View = Position.ToNDC();
        }

        public void ToScreen(Screen screen)
        {
            Position = Position.ToScreen(screen);
        }

        static public OutputVS Blend(OutputVS v1, OutputVS v2, float t)
        {
            var uvs = new List<Vector2>();
            int count = Math.Min(v1.UVs.Length, v2.UVs.Length);
            for(int i = 0; i < count; i++)
                uvs.Add(v1.UVs[i] * (1f - t) + v2.UVs[i] * t);
            return new OutputVS
            {
                Position = v1.position * (1f - t) + v2.position * t,
                Normal = v1.Normal * (1f - t) + v2.Normal * t,
                UVs = uvs.ToArray(),
                Color = v1.Color * (1f - t) + v2.Color * t,
            };
        }
    }

    public class InputPS
    {
        public Color Color { get; set; }
        public Texture[] Textures { get; set; }
        public List<Vector2> UVs { get; set; }
        public Vector3 Normal { get; set; }
        public Color VertexColor { get; set; }

        public Color GetSample(Material.Type type)
        {
            int index = (int)type;
            return Textures[index]?.GetSample(UVs[index % UVs.Count]) ?? Color.White;
        }
    }

    public class OutputPS
    {
        public Color Color { get; set; }
        public Vector3 Normal { get; set; }
    }
}
