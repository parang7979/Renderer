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
    public class GBuffer
    {
        public enum BufferType
        {
            Gizmo,
            Albedo,
            Position,
            Normal,
        }

        public bool IsLock => locks.Count > 0;
        public Bitmap RenderTarget => render;

        private Bitmap render;
        private Dictionary<BufferType, Bitmap> buffers = new Dictionary<BufferType, Bitmap>();
        private Dictionary<BufferType, BitmapData> locks = new Dictionary<BufferType, BitmapData>();

        public GBuffer(int width, int height)
        {
            render = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            foreach(BufferType t in Enum.GetValues(typeof(BufferType)))
            {
                buffers.Add(t, new Bitmap(width, height, PixelFormat.Format24bppRgb));
            }
            locks.Clear();
        }

        public void Lock()
        {
            if (locks.Count > 0) return;
            foreach(var b in buffers)
            {
                var l = b.Value.LockBits(
                        new Rectangle(0, 0, render.Width, render.Height),
                        ImageLockMode.ReadWrite, render.PixelFormat);
                l.Clear(Color.Black);
                locks.Add(b.Key, l);
            }
        }

        public void Clear()
        {
            if (locks.Count == 0) return;
            foreach (var l in locks) l.Value.Clear(Color.Black);
        }

        /* public Color GetPixel(BufferType type, int x, int y)
        {
            if (locks.Count == 0) return Color.Black;
            if (locks.ContainsKey(type))
            {
                var l = locks[type];
                return l.GetPixel(x, y);
            }
            return Color.White;
        } */

        public void DrawTriangle(Screen screen, Vertex v1, Vertex v2, Vertex v3, in Texture texture)
        {
            if (locks.Count == 0) return;

            var p1 = screen.ToPoint(v1);
            var p2 = screen.ToPoint(v2);
            var p3 = screen.ToPoint(v3);

            var min = screen.Clamp(new Point(
                MathExtension.Min3(p1.X, p2.X, p3.X),
                MathExtension.Min3(p1.Y, p2.Y, p3.Y))); // 좌상

            var max = screen.Clamp(new Point(
                MathExtension.Max3(p1.X, p2.X, p3.X),
                MathExtension.Max3(p1.Y, p2.Y, p3.Y))); // 우하

            var u = v2.Vector2 - v1.Vector2;
            var v = v3.Vector2 - v1.Vector2;
            float uDv = Vector2.Dot(u, v);
            float vDv = Vector2.Dot(v, v);
            float uDu = Vector2.Dot(u, u);
            var d = uDv * uDv - vDv * uDu;
            if (d == 0f) return;
            float invD = 1 / d;

            float invZ1 = 1f / v1.W;
            float invZ2 = 1f / v2.W;
            float invZ3 = 1f / v3.W;

            for (int x = min.X; x < max.X; x++)
            {
                for (int y = min.Y; y < max.Y; y++)
                {
                    Point p = new Point(x, y);
                    Vector2 w = p.ToVector2(screen) - v1.Vector2;
                    float wDu = Vector2.Dot(w, u);
                    float wDv = Vector2.Dot(w, v);
                    float s = (wDv * uDv - wDu * vDv) * invD;
                    float t = (wDu * uDv - wDv * uDu) * invD;
                    float o = 1f - s - t;
                    if ((0f <= s && s <= 1f) && (0f <= t && t <= 1f) && (0f <= o && o <= 1f))
                    {
                        var pos = v1.Vector3 * o + v2.Vector3 * s + v3.Vector3 * t;
                        if (!SetPositionBuffer(screen, x, y, pos)) continue;
                        var z = invZ1 * o + invZ2 * s + invZ3 * t;
                        var invZ = 1f / z;
                        // Normal
                        var normal = v1.Normal * o + v2.Normal * s + v3.Normal * t;
                        // Albedo
                        Color albedo;
                        if (texture != null)
                        {
                            var uv = (v1.UV * o * invZ1 + v2.UV * s * invZ2 + v3.UV * t * invZ3) * invZ;
                            albedo = texture.GetSample(uv);
                        }
                        else
                        {
                            albedo = (v1.Color * o * invZ1 + v2.Color * s * invZ2 + v3.Color * t * invZ3) * invZ;
                        }
                        // PS 에 노말 알베도 버텍스 컬러 전달?
                        SetNormalBuffer(x, y, normal);
                        SetAlbedoBuffer(x, y, albedo);
                    }
                }
            }
        }

        public void DrawWireframe(Screen screen, Vertex v1, Vertex v2, Vertex v3)
        {
            DrawLine(screen, v1, v2);
            DrawLine(screen, v2, v3);
            DrawLine(screen, v3, v1);
        }

        public void DrawLine(Screen screen, Vertex v1, Vertex v2)
        {
            if (locks.Count == 0) return;

            var p1 = screen.ToPoint(v1);
            var p2 = screen.ToPoint(v2);

            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;

            if (screen.ClipLine(ref p1, ref p2))
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
                        SetGizmoBuffer(x, y, c);
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
                        SetGizmoBuffer(x, y, c);
                    }
                }
            }
        }

        public void SetAlbedoBuffer(int x, int y, Color color)
        {
            if (locks.Count == 0) return;

            var b = locks[BufferType.Albedo];
            var index = y * b.Width + x;
            if (index < 0 || b.Width * b.Height <= index) return;
            index *= 3;
            unsafe
            {
                var ptr = (byte*)b.Scan0;
                ptr[index] = color.BB;
                ptr[index + 1] = color.BG;
                ptr[index + 2] = color.BR;
            }
        }

        public bool SetPositionBuffer(Screen screen, int x, int y, Vector3 pos)
        {
            if (locks.Count == 0) return false;

            var b = locks[BufferType.Position];
            var index = y * b.Width + x;
            if (index < 0 || b.Width * b.Height <= index) return false;
            index *= 3;
            unsafe
            {
                var ptr = (byte*)b.Scan0;
                byte cz = (byte)((1 - pos.Z) * 255f);
                byte pz = ptr[index];
                // depth testing
                if (pz < cz)
                {
                    ptr[index] = cz;
                    ptr[index + 1] = (byte)((pos.Y + screen.HalfHeight) / screen.Height * 255f);
                    ptr[index + 2] = (byte)((pos.X + screen.HalfWidth) / screen.Width * 255f);
                    return true;
                }
                return false;
            }
        }

        public void SetNormalBuffer(int x, int y, Vector3 normal)
        {
            if (locks.Count == 0) return;

            var b = locks[BufferType.Normal];
            var index = y * b.Width + x;
            if (index < 0 || b.Width * b.Height <= index) return;
            index *= 3;
            unsafe
            {
                var ptr = (byte*)b.Scan0;
                normal = Vector3.Normalize(normal);
                ptr[index] = (byte)((normal.Z + 1) / 2 * 255);
                ptr[index + 1] = (byte)((normal.Y + 1) / 2 * 255);
                ptr[index + 2] = (byte)((normal.X + 1) / 2 * 255);
            }
        }

        public void SetGizmoBuffer(int x, int y, Color color)
        {
            if (locks.Count == 0) return;

            var b = locks[BufferType.Gizmo];
            var index = y * b.Width + x;
            if (index < 0 || b.Width * b.Height <= index) return;
            index *= 3;
            unsafe
            {
                var ptr = (byte*)b.Scan0;
                ptr[index] = color.BB;
                ptr[index + 1] = color.BG;
                ptr[index + 2] = color.BR;
            }
        }

        public void Render(Color clearColor, List<Vector3> lights)
        {
            var l1 = new Color("red");
            var l2 = new Color("Green");
            var l3 = new Color("blue");

            if (locks.Count == 0) return;
            var l = render.LockBits(new Rectangle(0, 0, render.Width, render.Height),
                        ImageLockMode.ReadWrite, render.PixelFormat);
            l.Clear(clearColor);
            for (int y = 0; y < l.Height; y++)
            {
                for (int x = 0; x < l.Width; x++)
                {
                    var c1 = locks[BufferType.Albedo].GetPixel(x, y);
                    if (c1.IsBlack) continue;
                    var c2 = locks[BufferType.Gizmo].GetPixel(x, y);

                    var n = locks[BufferType.Normal].GetPixel(x, y);
                    if (!n.IsBlack)
                    {
                        var normal = 
                            new Vector3((n.R * 2f) - 1f, (n.G * 2f) - 1f, (n.B * 2f) - 1f);
                        var d1 = -Math.Min(0, Vector3.Dot(normal, lights[0]));
                        var d2 = -Math.Min(0, Vector3.Dot(normal, lights[1]));
                        var d3 = -Math.Min(0, Vector3.Dot(normal, lights[2]));
                        c1 *= c1 * ((l1 * d1) + (l2 * d2) + (l3 * d3));
                        c1 += c2;
                    }
                    // if (!c1.IsBlack) 
                        l.SetPixel(x, y, c1);
                }
            }
            render.UnlockBits(l);
        }

        public void Unlock()
        {
            if (locks.Count == 0) return;
            foreach (var b in buffers)
                b.Value.UnlockBits(locks[b.Key]);
            locks.Clear();
        }

        public Bitmap GetBuffer(BufferType type) => buffers[type];
    }
}
