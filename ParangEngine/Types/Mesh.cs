using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class Mesh
    {
        public int key { get; set; }
        
        private List<Vertex> vertices;
        private List<int> indicies;

        public Mesh(int key, List<Vertex> vertices, List<int> indicies)
        {
            this.key = key;
            this.vertices = vertices;
            this.indicies = indicies;
        }

        private float Min3(float a, float b, float c)
        {
            return Math.Min(a, Math.Min(b, c));
        }

        private float Max3(float a, float b, float c)
        {
            return Math.Max(a, Math.Max(b, c));
        }

        private void DrawPolygon(BitmapData b, Texture texture, Vertex v1, Vertex v2, Vertex v3)
        {
            var min = new Vector2(Min3(v1.X, v2.X, v3.X), Min3(v1.Y, v2.Y, v3.Y)); // 좌상
            var max = new Vector2(Max3(v1.X, v2.X, v3.X), Max3(v1.Y, v2.Y, v3.Y)); // 우하
            var u = v2.Pos.ToVector2() - v1.Pos.ToVector2();
            var v = v3.Pos.ToVector2() - v1.Pos.ToVector2();
            float uDv = Vector2.Dot(u, v);
            float vDv = Vector2.Dot(v, v);
            float uDu = Vector2.Dot(u, u);
            var d = uDv * uDv - vDv * uDu;
            if (d == 0f) return;
            float invD = 1 / d;
            var minPixel = new Pixel(min);
            var maxPixel = new Pixel(max);

            minPixel.X = Math.Max(0, Math.Min(minPixel.X, b.Width - 1));
            minPixel.Y = Math.Max(0, Math.Min(minPixel.Y, b.Height - 1));

            maxPixel.X = Math.Max(0, Math.Min(maxPixel.X, b.Width - 1));
            maxPixel.Y = Math.Max(0, Math.Min(maxPixel.Y, b.Height - 1));

            /* float invZ1 = 1 / v1.Pos.W;
            float invZ2 = 1 / v2.Pos.W;
            float invZ3 = 1 / v3.Pos.W; */

            for (int x = minPixel.X; x < maxPixel.X; x++)
            {
                for (int y = minPixel.Y; y < maxPixel.Y; y++)
                {
                    Vector2 w = new Vector2(x, y) - v1.Pos.ToVector2();
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

        private void DrawLine(BitmapData b, Vertex v1, Vertex v2)
        {
            var p1 = new Pixel(v1);
            var p2 = new Pixel(v2);

            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;

            if (Pixel.ClipLine(b, ref p1, ref p2))
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

        public void Draw(BitmapData b, System.Numerics.Matrix4x4 mat, Texture texture)
        {
            int count = indicies.Count / 3 * 3;
            for (int i = 0; i < count; i += 3)
            {
                var v1 = vertices[indicies[i]];
                var v2 = vertices[indicies[i + 1]];
                var v3 = vertices[indicies[i + 2]];
                DrawPolygon(b, texture, v1, v2, v3);
                DrawLine(b, v1, v2);
                DrawLine(b, v2, v3);
                DrawLine(b, v3, v1);
            }
        }
    }
}
