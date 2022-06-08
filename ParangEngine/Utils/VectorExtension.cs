using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    static public class VectorExtension
    {
        static public Vector2 ToVector2(this Vector4 v)
        {
            return new Vector2(v.X, v.Y);
        }

        static public Vector3 ToVector3(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
    }
}
