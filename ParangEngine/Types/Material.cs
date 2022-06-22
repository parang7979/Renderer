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
            Surface,
            Max,
        }
        public Color Color { get; set;}
        public float Metalic { get; set; }
        public float Roughness { get; set; }

        public List<Texture> Textures => textures.Where(x => x != null).ToList();

        private Texture[] textures = new Texture[(int)Type.Max];
        private BaseShader shader = new BaseShader();

        public OutputVS Convert(Vertex v, Matrix4x4 tMat, Matrix4x4 pvMat)
        {
            return shader.VertexShader(new InputVS
            {
                Position = v.Vector4,
                Normal = v.Normal,
                UV = v.UV,
                TMat = tMat,
                PVMat = pvMat,
                Color = v.Color,
            });
        }

        public OutputPS Convert(Vector2 uv, Vector3 normal, Quaternion normalQuat, Color vertexColor)
        {
            return shader.PixelShader(new InputPS
            {
                Color = Color,
                Metalic = Metalic,
                Roughness = Roughness,
                Textures = textures,
                UV = uv,
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

        public void SetShader(BaseShader shader)
        {
            this.shader = shader;
        }
    }
}
