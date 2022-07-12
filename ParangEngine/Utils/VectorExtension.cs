using ParangEngine.Types;
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
        
        static public Vector4 ToNDC(this Vector4 v)
        {
            v.W = v.W == 0f ? 0.0001f : v.W;
            var invW = 1f / v.W;
            v.X *= invW;
            v.Y *= invW;
            v.Z *= invW;
            return v;
        }

        static public Vector4 ToInvNDC(this Vector4 v)
        {
            var W = v.W;
            v.X *= W;
            v.Y *= W;
            v.Z *= W;
            return v;
        }

        static public Vector4 ToScreen(this Vector4 v, Screen screen)
        {
            v.X *= screen.HalfWidth;
            v.Y *= screen.HalfHeight;
            return v;
        }

        static public Vector4 ToInvScreen(this Vector4 v, Screen screen)
        {
            v.X /= screen.HalfWidth;
            v.Y /= screen.HalfHeight;
            return v;
        }

        static public Vector3 ToVector3(this string[] strs, int startIndex)
        {
            if (strs.Length < startIndex + 3) return Vector3.Zero;
            return new Vector3(
                strs[startIndex].SafeParse(0f),
                strs[startIndex + 1].SafeParse(0f),
                strs[startIndex + 2].SafeParse(0f));
        }

        static public Vector2 ToVector2(this string[] strs, int startIndex)
        {
            if (strs.Length < startIndex + 2) return Vector2.Zero;
            return new Vector2(
                strs[startIndex].SafeParse(0f),
                strs[startIndex + 1].SafeParse(0f));
        }

        static public float SafeParse(this string str, float def)
        {
            if (float.TryParse(str, out var ret)) return ret;
            return def;
        }

        static public int SafeParse(this string str, int def)
        {
            if (int.TryParse(str, out var ret)) return ret;
            return def;
        }

        static public Quaternion RotateTo(Vector3 from, Vector3 to)
        {
            return GetRotate(to - from, Vector3.UnitZ);
        }

        static public Quaternion GetRotate(Vector3 v1, Vector3 v2)
        {
            v1 = Vector3.Normalize(v1);
            v2 = Vector3.Normalize(v2);
            return Quaternion.CreateFromAxisAngle(
                Vector3.Cross(v1, v2),
                (float)Math.Acos(Vector3.Dot(v1, v2)));
        }

        static public Vector3 ToEuler(this Quaternion q)
        {
            Vector3 angles;

            // roll (x-axis rotation)
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.Y = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
                angles.Z = Math.Sign(sinp) * MathExtension.PI / 2; // use 90 degrees if out of range
            else
                angles.Z = (float)Math.Asin(sinp);

            // yaw (z-axis rotation)
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.X = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }
    }
}
