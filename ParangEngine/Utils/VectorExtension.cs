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

        static public Vector4 Clip(this Vector4 v1, Vector4 v2)
        {
            if (v1.W < 0)
            {
                var p1 = v1.W;
                var p2 = v2.W;
                var t = p1 / (p1 - p2);
                v1 = v1 * (1f - t) + v2 * t;
            }

            if (v1.Y > v1.W)
            {
                var p1 = v1.W - v1.Y;
                var p2 = v2.W - v2.Y;
                var t = p1 / (p1 - p2);
                v1 = v1 * (1 - t) + v2 * t;
            }

            if (v1.Y < -v1.W)
            {
                var p1 = v1.W + v1.Y;
                var p2 = v2.W + v2.Y;
                var t = p1 / (p1 - p2);
                v1 = v1 * (1 - t) + v2 * t;
            }

            if (v1.X > v1.W)
            {
                var p1 = v1.W - v1.X;
                var p2 = v2.W - v2.X;
                var t = p1 / (p1 - p2);
                v1 = v1 * (1 - t) + v2 * t;
            }

            if (v1.X < -v1.W)
            {
                var p1 = v1.W + v1.X;
                var p2 = v2.W + v2.X;
                var t = p1 / (p1 - p2);
                v1 = v1 * (1 - t) + v2 * t;
            }

            if (v1.Z > v1.W)
            {
                var p1 = v1.W - v1.Z;
                var p2 = v2.W - v2.Z;
                var t = p1 / (p1 - p2);
                v1 = v1 * (1 - t) + v2 * t;
            }

            if (v1.Z < -v1.W)
            {
                var p1 = v1.W + v1.Z;
                var p2 = v2.W + v2.Z;
                var t = p1 / (p1 - p2);
                v1 = v1 * (1 - t) + v2 * t;
            }

            return v1;
        }

        static public Plane ToPlane(this Vector3 normal, Vector3 v)
        {
            return new Plane(normal, -Vector3.Dot(normal, v));
        }
    }
}
