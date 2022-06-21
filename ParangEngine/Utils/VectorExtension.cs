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
    }
}
