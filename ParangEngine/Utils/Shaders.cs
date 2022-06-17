using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    static public class Shaders
    {        
        static public OutputVS DefaultVS(InputVS input)
        {
            return new OutputVS()
            {
                Position = Vector4.Transform(input.Position, input.TMat * input.PVMat),
                Normal = Vector4.Transform(new Vector4(input.Normal, 0), input.TMat).ToVector3(),
                UVs = input.UVs.ToArray(),
                Color = input.Color,
            };
        }

        static private Vector3 UnpackNormal(Color n, Quaternion normalQuat)
        {
            return Vector3.Transform(new Vector3(n.R * 2 - 1, n.G * 2 - 1, n.B * 2 - 1), normalQuat);
        }

        static public OutputPS DefaultPS(InputPS input)
        {
            return new OutputPS
            {
                Color = input.GetSample(Material.Type.Albedo),
                Normal = UnpackNormal(input.GetSample(Material.Type.Normal), input.RotNormal),
            };
        }
    }
}
