using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    public class BaseShader
    {
        virtual public OutputVS VertexShader(InputVS input)
        {
            return new OutputVS()
            {
                Position = Vector4.Transform(input.Position, input.TMat * input.PVMat),
                Normal = Vector3.TransformNormal(input.Normal, input.TMat),
                UVs = input.UVs.ToArray(),
                Color = input.Color,
            };
        }

        protected Vector3 UnpackNormal(Color n, Quaternion normalQuat)
        {
            return Vector3.Transform(new Vector3(n.R * 2 - 1, n.G * 2 - 1, n.B * 2 - 1), normalQuat);
        }

        virtual public OutputPS PixelShader(InputPS input)
        {
            var surface = input.GetSample(Material.Type.Surface);
            surface.R *= input.Metalic;
            surface.G *= input.Roughness;
            return new OutputPS
            {
                Color = input.GetSample(Material.Type.Albedo),
                Surface = surface,
                Normal = UnpackNormal(input.GetSample(Material.Type.Normal), input.RotNormal),
            };
        }
    }
}
