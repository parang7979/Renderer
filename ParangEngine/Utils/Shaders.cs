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
                UVs = input.UVs,
            };
        }

        static private Vector3 UnpackNormal(Color n)
        {
            var rot = Quaternion.CreateFromYawPitchRoll(-MathExtension.HalfPI, -MathExtension.HalfPI, 0f);
            return Vector3.Transform(new Vector3(n.R * 2 - 1, n.G * 2 - 1, n.B * 2 - 1), rot);
        }

        static public OutputPS DefaultPS(InputPS input)
        {
            return new OutputPS
            {
                Color = Color.White,//input.GetSample(Material.Type.Albedo),
                Normal = UnpackNormal(input.GetSample(Material.Type.Normal)),
            };
        }
    }
}
