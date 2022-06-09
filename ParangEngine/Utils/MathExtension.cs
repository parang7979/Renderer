using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    static public class MathExtension
    {
        public static readonly float PI = (float)Math.PI;
        public static readonly float TwoPI = PI * 2f;
        public static readonly float HalfPI = PI * 0.5f;
        public static readonly float InvPI = 1f / PI;

        public static float ToRad(this float deg)
        {
            return deg / 180f * PI;
        }

        public static float ToDeg(float rad)
        {
            return rad * 180 * InvPI;
        }

        public static (float sin, float cos) GetSinCosRad(this float rad)
        {
            return ((float)Math.Sin(rad), (float)Math.Cos(rad));
        }

        public static (float sin, float cos) GetSinCos(this float degree)
        {
            if (degree == 0f) return (0f, 1f);
            else if (degree == 90f) return (1f, 0f);
            else if (degree == 180f) return (0f, -1f);
            else if (degree == 270f) return (-1f, 0f);
            else return GetSinCosRad(degree);
        }

        public static Matrix4x4 GetMatrix(Vector4 v1, Vector4 v2, Vector4 v3, Vector4 v4)
        {
            return new Matrix4x4(
                v1.X, v1.Y, v1.Z, v1.W,
                v2.X, v2.Y, v2.Z, v2.W,
                v3.X, v3.Y, v3.Z, v3.W,
                v4.X, v4.Y, v4.Z, v4.W);
        }
    }
}
