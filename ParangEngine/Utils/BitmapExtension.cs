﻿using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    static public class BitmapExtension
    {
        static public void SetPixel(this BitmapData b, int index, in Color color)
        {
            if (index < 0 || b.Width * b.Height <= index) return;
            unsafe
            {
                var ptr = (byte*)b.Scan0;
                index *= 3;
                ptr[index] = color.BB;
                ptr[index + 1] = color.BG;
                ptr[index + 2] = color.BR;
            }
        }

        static public void SetPixel(this BitmapData b, int x, int y, in Color color)
        {
            SetPixel(b, y * b.Width + x, color);
        }

        static public void Clear(this BitmapData b, in Color color)
        {
            for (int i = 0; i < b.Height * b.Width; i++) SetPixel(b, i, color);
        }

        static public void Blend(this BitmapData dst, in BitmapData src1, in BitmapData src2)
        {
            unsafe
            {
                var d = (byte*)dst.Scan0;
                var p1 = (byte*)src1.Scan0;
                var p2 = (byte*)src2.Scan0;

                for (int i = 0; i < dst.Height * dst.Width * 3; i++)
                {
                    d[i] = (byte)((p1[i] / 255f) * (p2[i] / 255f) * 255f);
                }
            }
        }

        static public int Min3(int a, int b, int c)
        {
            return Math.Min(a, Math.Min(b, c));
        }

        static public int Max3(int a, int b, int c)
        {
            return Math.Max(a, Math.Max(b, c));
        }

        static public void DrawPolygon(this BitmapData b, in Screen size, in Vertex v1, in Vertex v2, in Vertex v3, in Texture texture)
        {
            var p1 = new Point(size, v1);
            var p2 = new Point(size, v2);
            var p3 = new Point(size, v3);

            var min = size.Clamp(new Point(
                Min3(p1.X, p2.X, p3.X),
                Min3(p1.Y, p2.Y, p3.Y))); // 좌상

            var max = size.Clamp(new Point(
                Max3(p1.X, p2.X, p3.X),
                Max3(p1.Y, p2.Y, p3.Y))); // 우하

            var u = v2.Pos.ToVector2() - v1.Pos.ToVector2();
            var v = v3.Pos.ToVector2() - v1.Pos.ToVector2();
            float uDv = Vector2.Dot(u, v);
            float vDv = Vector2.Dot(v, v);
            float uDu = Vector2.Dot(u, u);
            var d = uDv * uDv - vDv * uDu;
            if (d == 0f) return;
            float invD = 1 / d;

            /* float invZ1 = 1 / v1.Pos.W;
            float invZ2 = 1 / v2.Pos.W;
            float invZ3 = 1 / v3.Pos.W; */

            for (int x = min.X; x < max.X; x++)
            {
                for (int y = min.Y; y < max.Y; y++)
                {
                    Point p = new Point(x, y);
                    Vector2 w = p.ToVector2(size) - v1.Pos.ToVector2();
                    float wDu = Vector2.Dot(w, u);
                    float wDv = Vector2.Dot(w, v);
                    float s = (wDv * uDv - wDu * vDv) * invD;
                    float t = (wDu * uDv - wDv * uDu) * invD;
                    float o = 1f - s - t;
                    if ((0f <= s && s <= 1f) && (0f <= t && t <= 1f) && (0f <= o && o <= 1f))
                    {
                        var c = v1.Color * o + v2.Color * s + v3.Color * t;
                        if (texture != null)
                        {
                            var uv = v1.UV * o + v2.UV * s + v3.UV * t;
                            c = texture.GetSample(uv);
                        }
                        b.SetPixel(x, y, c);
                    }
                }
            }
        }

        static public void DrawLine(this BitmapData b, in Screen size, in Vertex v1, in Vertex v2)
        {
            var p1 = new Point(size, v1);
            var p2 = new Point(size, v2);

            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;

            if (size.ClipLine(ref p1, ref p2))
            {
                if (Math.Abs(dx) < Math.Abs(dy))
                {
                    var t1 = p2.Y < p1.Y ? p2 : p1;
                    var t2 = p2.Y < p1.Y ? p1 : p2;

                    var c1 = p2.Y < p1.Y ? v2.Color : v1.Color;
                    var c2 = p2.Y < p1.Y ? v1.Color : v2.Color;

                    for (int y = t1.Y; y < t2.Y; y++)
                    {
                        float w = (y - t1.Y) / (float)dy;
                        int x = dy != 0 ? t1.X + (int)(dx * w) : 0;
                        w = Math.Abs(w);
                        var c = (c2 * w) + (c1 * (1 - w));
                        b.SetPixel(x, y, c);
                    }
                }
                else
                {
                    var t1 = p2.X < p1.X ? p2 : p1;
                    var t2 = p2.X < p1.X ? p1 : p2;

                    var c1 = p2.X < p1.X ? v2.Color : v1.Color;
                    var c2 = p2.X < p1.X ? v1.Color : v2.Color;

                    for (int x = t1.X; x < t2.X; x++)
                    {
                        float w = (x - t1.X) / (float)dx;
                        int y = dx != 0 ? t1.Y + (int)(dy * w) : 0;
                        w = Math.Abs(w);
                        var c = (c2 * w) + (c1 * (1 - w));
                        b.SetPixel(x, y, c);
                    }
                }
            }
        }

        static public void DrawLine(this BitmapData b, in Screen size, in Vector4 v1, in Vector4 v2, in Color color)
        {
            var p1 = new Point(size, v1);
            var p2 = new Point(size, v2);

            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;

            if (size.ClipLine(ref p1, ref p2))
            {
                if (Math.Abs(dx) < Math.Abs(dy))
                {
                    var t1 = p2.Y < p1.Y ? p2 : p1;
                    var t2 = p2.Y < p1.Y ? p1 : p2;

                    for (int y = t1.Y; y < t2.Y; y++)
                    {
                        float w = (y - t1.Y) / (float)dy;
                        int x = dy != 0 ? t1.X + (int)(dx * w) : 0;
                        b.SetPixel(x, y, color);
                    }
                }
                else
                {
                    var t1 = p2.X < p1.X ? p2 : p1;
                    var t2 = p2.X < p1.X ? p1 : p2;

                    for (int x = t1.X; x < t2.X; x++)
                    {
                        float w = (x - t1.X) / (float)dx;
                        int y = dx != 0 ? t1.Y + (int)(dy * w) : 0;
                        b.SetPixel(x, y, color);
                    }
                }
            }
        }
    }
}
