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

        static public int Min3(int a, int b, int c)
        {
            return Math.Min(a, Math.Min(b, c));
        }

        static public int Max3(int a, int b, int c)
        {
            return Math.Max(a, Math.Max(b, c));
        }

        public static float ToRad(this float deg)
        {
            return deg / 180f * PI;
        }

        public static float ToDeg(float rad)
        {
            return rad * 180 * InvPI;
        }

        public static Matrix4x4 MakeMatrix(Vector4 v1, Vector4 v2, Vector4 v3, Vector4 v4)
        {
            return new Matrix4x4(
                v1.X, v1.Y, v1.Z, v1.W,
                v2.X, v2.Y, v2.Z, v2.W,
                v3.X, v3.Y, v3.Z, v3.W,
                v4.X, v4.Y, v4.Z, v4.W);
        }

        public static Vector4[] SplitMatrix(this Matrix4x4 mat)
        {
            return new Vector4[]
            {
                new Vector4(mat.M11, mat.M12, mat.M13, mat.M14),
                new Vector4(mat.M21, mat.M22, mat.M23, mat.M24),
                new Vector4(mat.M31, mat.M32, mat.M33, mat.M34),
                new Vector4(mat.M41, mat.M42, mat.M43, mat.M44),
            };
        }

        static public float Distance(this Plane plane, Vector3 v)
        {
            return Vector3.Dot(plane.Normal, v) + plane.D;
        }

        static public bool IsOutside(this Plane plane, Vector3 v)
        {
            return plane.Distance(v) > 0f;
        }
    }
}
