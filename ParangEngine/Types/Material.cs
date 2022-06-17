using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class Material
    {
        public enum Type
        {
            Albedo,
            Normal,
            Max,
        }
        public int Id { get; private set; }
        public Color Color { get; private set;}

        private Texture[] textures = new Texture[(int)Type.Max];
        private Func<InputVS, OutputVS> vs = Shaders.DefaultVS;
        private Func<InputPS, OutputPS> ps = Shaders.DefaultPS;

        public Material(int id)
        {
            Id = id;
        }

        public OutputVS Convert(Vertex v, Matrix4x4 tMat, Matrix4x4 pvMat)
        {
            return vs.Invoke(new InputVS
            {
                Position = v.Vector4,
                Normal = v.Normal,
                UVs = new Vector2[] { v.UV, v.UV },
                TMat = tMat,
                PVMat = pvMat,
            });
        }

        public OutputPS Convert(List<Vector2> uvs, Vector3 normal, Quaternion normalQuat, Color vertexColor)
        {
            return ps.Invoke(new InputPS
            {
                Color = Color,
                Textures = textures,
                UVs = uvs.ToArray(),
                Normal = normal,
                RotNormal = normalQuat,
                VertexColor = vertexColor,
            });
        }

        public void AddTexture(Type type, Texture texture)
        {
            textures[(int)type] = texture;
        }

        public void RemoveTexture(Type type)
        {
            textures[(int)type] = null;
        }

        public void Lock()
        {
            foreach (var t in textures) t?.Lock();
        }

        public void Unlock()
        {
            foreach (var t in textures) t?.Unlock();
        }
    }
}
