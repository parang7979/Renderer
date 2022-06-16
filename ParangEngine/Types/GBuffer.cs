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
            Albedo,
            Position,
            Normal,
            Specular,
            Gizmo,
        }

        public bool IsLock => locks.Count > 0;
        public Bitmap RenderTarget => render;

        private int spliter = 8;
        private Bitmap render;
        private Dictionary<BufferType, Bitmap> buffers = new Dictionary<BufferType, Bitmap>();
        private Dictionary<BufferType, BitmapData> locks = new Dictionary<BufferType, BitmapData>();

        public GBuffer(int width, int height)
        {
            render = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            buffers.Add(BufferType.Albedo, new Bitmap(width, height, PixelFormat.Format24bppRgb));
            buffers.Add(BufferType.Position, new Bitmap(width, height, PixelFormat.Format64bppArgb));
            buffers.Add(BufferType.Normal, new Bitmap(width, height, PixelFormat.Format48bppRgb));
            buffers.Add(BufferType.Specular, new Bitmap(width, height, PixelFormat.Format48bppRgb));
            buffers.Add(BufferType.Gizmo, new Bitmap(width, height, PixelFormat.Format24bppRgb));
            locks.Clear();
        }

        public void Lock()
        {
            if (locks.Count > 0) return;
            foreach(var b in buffers)
            {
                var l = b.Value.LockBits(
                        new Rectangle(0, 0, b.Value.Width, b.Value.Height),
                        ImageLockMode.ReadWrite, b.Value.PixelFormat);
                l.Clear(Color.Black);
                locks.Add(b.Key, l);
            }
        }

        public void DrawTriangle(Screen screen, Vertex v1, Vertex v2, Vertex v3, in Texture texture, Color color)
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
                        var z = invZ1 * o + invZ2 * s + invZ3 * t;
                        var invZ = 1f / z;
                        var uv = (v1.UV * o * invZ1 + v2.UV * s * invZ2 + v3.UV * t * invZ3) * invZ;

                        // position
                        /* var pos = Vertex.ToView(v1, screen).Vector3 * o +
                            Vertex.ToView(v2, screen).Vector3 * s + 
                            Vertex.ToView(v3, screen).Vector3 * t; */
                        var pos = v1.View * o + v2.View * s + v3.View * t;
                        if (!SetPositionBuffer(screen, x, y, pos)) continue;
                        
                        // Normal
                        var normal = v1.Normal * o + v2.Normal * s + v3.Normal * t;
                        var n = texture.GetSample(uv);
                        var normalT = new Vector3();
                        normalT.X = n.R * 2 - 1;
                        normalT.Y = n.G * 2 - 1;
                        normalT.Z = n.B * 2 - 1;
                        SetNormalBuffer(x, y, Vector3.Normalize(normal - normalT));

                        // Albedo
                        Color albedo;
                        if (texture != null)
                        {
                            albedo = color;// texture.GetSample(uv) * color;
                        }
                        else
                        {
                            albedo = (v1.Color * o * invZ1 + v2.Color * s * invZ2 + v3.Color * t * invZ3) * invZ;
                        }
                        SetAlbedoBuffer(x, y, albedo);
                        // SetSpecularBuffer(x, y, normal);
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

        public bool SetPositionBuffer(Screen screen, int x, int y, Vector4 pos)
        {
            if (locks.Count == 0) return false;

            var b = locks[BufferType.Position];
            var index = y * b.Width + x;
            if (index < 0 || b.Width * b.Height <= index) return false;
            index *= 4;
            unsafe
            {
                var ptr = (ushort*)b.Scan0;
                if (pos.Z < 0) return false;
                var cz = (ushort)((1 - pos.Z) * ushort.MaxValue / spliter);
                var pz = ptr[index];
                // depth testing
                if (pz < cz)
                {
                    ptr[index] = cz;
                    ptr[index + 1] = (ushort)((pos.Y + 1f) / 2f * ushort.MaxValue / spliter);
                    ptr[index + 2] = (ushort)((pos.X + 1f) / 2f * ushort.MaxValue / spliter);
                    ptr[index + 3] = (ushort)(pos.W / screen.ViewDistance * ushort.MaxValue / spliter);
                    return true;
                }
                return false;
            }
        }

        public bool CheckPositionBuffer(Screen screen, int x, int y, Vector4 pos)
        {
            if (locks.Count == 0) return false;

            var b = locks[BufferType.Position];
            var index = y * b.Width + x;
            if (index < 0 || b.Width * b.Height <= index) return false;
            index *= 3;
            unsafe
            {
                var ptr = (ushort*)b.Scan0;
                var cz = (ushort)(pos.Z / pos.W * ushort.MaxValue);
                var pz = ptr[index];
                // depth testing
                if (pz < cz)
                {
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
                var ptr = (ushort*)b.Scan0;
                normal = Vector3.Normalize(normal);
                ptr[index] = (ushort)((normal.Z + 1) / 2 * ushort.MaxValue / spliter);
                ptr[index + 1] = (ushort)((normal.Y + 1) / 2 * ushort.MaxValue / spliter);
                ptr[index + 2] = (ushort)((normal.X + 1) / 2 * ushort.MaxValue / spliter);
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

        public void SetSpecularBuffer(int x, int y, Vector3 normal)
        {
            if (locks.Count == 0) return;

            var b = locks[BufferType.Specular];
            var index = y * b.Width + x;
            if (index < 0 || b.Width * b.Height <= index) return;
            index *= 3;
            unsafe
            {
                var ptr = (ushort*)b.Scan0;
                var reflect = Vector3.Normalize(Vector3.Reflect(Vector3.UnitZ, normal));
                ptr[index] = (ushort)((reflect.Z + 1) / 2 * ushort.MaxValue);
                ptr[index + 1] = (ushort)((reflect.Y + 1) / 2 * ushort.MaxValue);
                ptr[index + 2] = (ushort)((reflect.X + 1) / 2 * ushort.MaxValue);
            }
        }

        public void Render(Screen screen, Color clearColor, List<Light> lights)
        {
            if (locks.Count == 0) return;
            var b = render.LockBits(new Rectangle(0, 0, render.Width, render.Height),
                        ImageLockMode.ReadWrite, render.PixelFormat);
            b.Clear(clearColor);
            for (int y = 0; y < b.Height; y++)
            {
                for (int x = 0; x < b.Width; x++)
                {
                    // 알베도
                    var a = locks[BufferType.Albedo].GetPixel(x, y);
                    var albedo = a;
                    // 포지션 버퍼가 없으면 아무것도 없는 공간
                    var p = locks[BufferType.Position].GetPixel(x, y);
                    if (!p.IsBlack)
                    {
                        var pos = new Vector4(p.R * spliter * 2f - 1f, p.G * spliter * 2f - 1f, 1 - p.B * spliter, p.A * spliter * screen.ViewDistance);
                        var n = locks[BufferType.Normal].GetPixel(x, y);
                        if (!n.IsBlack)
                        {
                            var normal =
                                new Vector3((n.R * spliter * 2f) - 1f, (n.G * spliter * 2f) - 1f, (n.B  * spliter * 2f) - 1f);
                            var l = Color.Black;
                            foreach (var light in lights)
                                l += light.GetColor(pos, normal);
                            albedo *= l;
                        }

                        /* var s = locks[BufferType.Specular].GetPixel(x, y);
                        if (!s.IsBlack && !albedo.IsBlack)
                        {
                            var specular =
                                    new Vector3((s.R * 2f) - 1f, (s.G * 2f) - 1f, (s.B * 2f) - 1f);
                            var l = Color.Black;
                            foreach (var light in lights)
                                l += light.GetSpecular(pos, a, specular);
                            albedo += l;
                        } */
                    }
                    // var gizmo = locks[BufferType.Gizmo].GetPixel(x, y);
                    // albedo = !gizmo.IsBlack ? gizmo : albedo;
                    b.SetPixel(x, y, albedo);
                }
            }
            render.UnlockBits(b);
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
