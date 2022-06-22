using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public partial class GBuffer
    {
        public enum BufferType
        {
            Albedo,
            Position,
            Normal,
            Surface,
        }

        public bool IsLock => locks.Count > 0;
        public Bitmap RenderTarget => render;

        private int renderSegment;
        private ushort precision = 8;
        private ushort maxUShort;

        private Bitmap render;
        private Dictionary<BufferType, Bitmap> buffers = new Dictionary<BufferType, Bitmap>();
        private Dictionary<BufferType, BitmapData> locks = new Dictionary<BufferType, BitmapData>();

        public GBuffer(int width, int height)
        {
            maxUShort = (ushort)(ushort.MaxValue / precision);
            render = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            buffers.Add(BufferType.Albedo, new Bitmap(width, height, PixelFormat.Format24bppRgb));
            buffers.Add(BufferType.Position, new Bitmap(width, height, PixelFormat.Format64bppArgb));
            buffers.Add(BufferType.Normal, new Bitmap(width, height, PixelFormat.Format48bppRgb));
            buffers.Add(BufferType.Surface, new Bitmap(width, height, PixelFormat.Format48bppRgb));
            locks.Clear();
            renderSegment = height / 10;
        }

        public void Lock(bool clear)
        {
            if (locks.Count > 0) return;
            foreach(var b in buffers)
            {
                var l = b.Value.LockBits(
                        new Rectangle(0, 0, b.Value.Width, b.Value.Height),
                        ImageLockMode.ReadWrite, b.Value.PixelFormat);
                if (clear) l.Clear(Color.Black);
                locks.Add(b.Key, l);
            }
        }

        private void DrawPixel(
            Screen screen, List<OutputVS> verticies, Material material, 
            Vector2 u, Vector2 v, Vector4 dots, Vector3 invZs,
            int x, int y)
        {
            Point p = new Point(x, y);
            Vector2 w = p.ToVector2(screen) - verticies[0].Vector2;
            float wDu = Vector2.Dot(w, u);
            float wDv = Vector2.Dot(w, v);
            float s = (wDv * dots.X - wDu * dots.Y) * dots.W;
            float t = (wDu * dots.X - wDv * dots.Z) * dots.W;
            float o = 1f - s - t;
            if ((0f <= s && s <= 1f) && (0f <= t && t <= 1f) && (0f <= o && o <= 1f))
            {
                using(new StopWatch("Gbuffer.DrawPixel"))
                {
                    // position
                    var pos = verticies[0].View * o + verticies[1].View * s + verticies[2].View * t;
                    // output buffer and depth testing
                    if (!SetPositionBuffer(screen, x, y, pos)) return;

                    var z = invZs.X * o + invZs.Y * s + invZs.Z * t;
                    var invZ = 1f / z;

                    // uvs
                    var uv = (verticies[0].UV * o * invZs.X + verticies[1].UV * s * invZs.Y + verticies[2].UV * t * invZs.Z) * invZ;

                    // normal
                    // var normal = Verticies[0].Normal * o + Verticies[1].Normal * s + Verticies[2].Normal * t;
                    var rotNormal = verticies[0].RotNormal * o + verticies[1].RotNormal * s + verticies[2].RotNormal * t;

                    // vertex color
                    // Color vertexColor = (Verticies[0].Color * o * InvZs.X + Verticies[1].Color * s * InvZs.Y + Verticies[2].Color * t * InvZs.Z) * invZ;
                    var output = material.Convert(uv, Vector3.Zero, rotNormal, Color.White);

                    // output buffer
                    SetNormalBuffer(x, y, Vector3.Normalize(output.Normal));
                    SetAlbedoBuffer(x, y, output.Color);
                    SetSurfaceBuffer(x, y, output.Surface);
                }
            }
        }

        public void DrawTriangle(Screen screen, List<OutputVS> vertices, Material material)
        {            
            if (locks.Count == 0) return;
            using (new StopWatch("GBuffer.DrawTriangle"))
            {
                var p1 = screen.ToPoint(vertices[0].Position);
                var p2 = screen.ToPoint(vertices[1].Position);
                var p3 = screen.ToPoint(vertices[2].Position);

                var min = screen.Clamp(new Point(
                    MathExtension.Min3(p1.X, p2.X, p3.X),
                    MathExtension.Min3(p1.Y, p2.Y, p3.Y))); // 좌상

                var max = screen.Clamp(new Point(
                    MathExtension.Max3(p1.X, p2.X, p3.X),
                    MathExtension.Max3(p1.Y, p2.Y, p3.Y))); // 우하

                var u = vertices[1].Vector2 - vertices[0].Vector2;
                var v = vertices[2].Vector2 - vertices[0].Vector2;
                Vector4 dots =
                    new Vector4(Vector2.Dot(u, v), Vector2.Dot(v, v), Vector2.Dot(u, u), 0);
                var d = dots.X * dots.X - dots.Y * dots.Z;
                if (d == 0f) return;
                dots.W = 1 / d;
                Vector3 invZs = new Vector3(1f / vertices[0].W, 1f / vertices[1].W, 1f / vertices[2].W);
                var w = max.X - min.X + 1;
                var h = max.Y - min.Y + 1;
                Parallel.For(0, w * h, (i) =>
                {
                    // if (i % 2 == 0)
                    {
                        var x = min.X + (i % w);
                        var y = min.Y + (i / w);
                        DrawPixel(screen, vertices, material, u, v, dots, invZs, x, y);
                    }
                });
            }
        }

        public void DrawWireframe(Screen screen, OutputVS v1, OutputVS v2, OutputVS v3)
        {
            DrawLine(screen, v1, v2);
            DrawLine(screen, v2, v3);
            DrawLine(screen, v3, v1);
        }

        public void DrawLine(Screen screen, OutputVS v1, OutputVS v2)
        {
            if (locks.Count == 0) return;

            var p1 = screen.ToPoint(v1.Position);
            var p2 = screen.ToPoint(v2.Position);

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
                        SetAlbedoBuffer(x, y, c);
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
                        SetAlbedoBuffer(x, y, c);
                    }
                }
            }
        }

        private void SetAlbedoBuffer(int x, int y, Color color)
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

        private bool SetPositionBuffer(Screen screen, int x, int y, Vector4 pos)
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
                var cz = (ushort)((1 - pos.Z) * ushort.MaxValue / precision);
                var pz = ptr[index];
                // depth testing
                if (pz < cz)
                {
                    ptr[index] = cz;
                    ptr[index + 1] = (ushort)((pos.Y + 1f) / 2f * ushort.MaxValue / precision);
                    ptr[index + 2] = (ushort)((pos.X + 1f) / 2f * ushort.MaxValue / precision);
                    ptr[index + 3] = (ushort)(pos.W / screen.ViewDistance * ushort.MaxValue / precision);
                    return true;
                }
                return false;
            }
        }

        private void SetNormalBuffer(int x, int y, Vector3 normal)
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
                ptr[index] = (ushort)((normal.Z + 1) / 2 * maxUShort);
                ptr[index + 1] = (ushort)((normal.Y + 1) / 2 * maxUShort);
                ptr[index + 2] = (ushort)((normal.X + 1) / 2 * maxUShort);
            }
        }

        public void SetSurfaceBuffer(int x, int y, Color color)
        {
            if (locks.Count == 0) return;

            var b = locks[BufferType.Surface];
            var index = y * b.Width + x;
            if (index < 0 || b.Width * b.Height <= index) return;
            index *= 3;
            unsafe
            {
                var ptr = (ushort*)b.Scan0;
                ptr[index] = color.SB;
                ptr[index + 1] = color.SG;
                ptr[index + 2] = color.SR;
            }
        }

        private void RenderPixel(BitmapData bitmap, Screen screen, 
            Vector3 view, Matrix4x4 invPvMat, List<Light> lights, 
            int x, int y)
        {
            // 알베도
            var a = locks[BufferType.Albedo].GetPixel(x, y);
            // 포지션 버퍼가 없으면 아무것도 없는 공간
            var p = locks[BufferType.Position].GetPixel(x, y);
            if (!p.IsBlack)
            {
                p *= precision;
                var pos = new Vector4(
                    p.R * 2f - 1f,
                    p.G * 2f - 1f,
                    1 - p.B,
                    p.A * screen.ViewDistance);

                // 위치를 월드 좌표계로 이동함
                var invPos = Vector4.Transform(pos.ToInvNDC(), invPvMat);
                // 법선
                var n = locks[BufferType.Normal].GetPixel(x, y);
                // 법선이 없어도 컬러는 그린다
                if (!n.IsBlack)
                {
                    n *= precision;
                    var normal = new Vector3(
                        (n.R * 2f) - 1f,
                        (n.G * 2f) - 1f,
                        (n.B * 2f) - 1f);
                    var surface = locks[BufferType.Surface].GetPixel(x, y);
                    var l = Color.Black;
                    foreach (var light in lights)
                        l += light.GetLight(invPos.ToVector3(), view, normal, surface);
                    a *= l;
                }
            }
            bitmap.SetPixel(x, y, a);
        }

        private void RenderPixels(BitmapData bitmap, Screen screen,
            Vector3 view, Matrix4x4 invPvMat, List<Light> lights,
            int minX, int maxX, int minY, int maxY)
        {
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    RenderPixel(bitmap, screen, view, invPvMat, lights, x, y);
                }
            }
        }

        private void SmoothPixel(BitmapData bitmap, int x, int y)
        {
            var p = locks[BufferType.Position].GetPixel(x, y);
            if (p.IsBlack)
            {
                var c1 = bitmap.GetPixel(x + 1, y);
                var c2 = bitmap.GetPixel(x, y + 1);
                var c3 = bitmap.GetPixel(x - 1, y);
                var c4 = bitmap.GetPixel(x, y - 1);
                bitmap.SetPixel(x, y, (c1 + c2 + c3 + c4) / 4);
            }
        }

        private void SmoothPixels(BitmapData bitmap,
            int minX, int maxX, int minY, int maxY)
        {
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    SmoothPixel(bitmap, x, y);
                }
            }
        }

        public void Render(Screen screen, Color clearColor, Vector3 view, Matrix4x4 pvMat, List<Light> lights)
        {
            if (locks.Count == 0) return;
            var b = render.LockBits(new Rectangle(0, 0, render.Width, render.Height),
                        ImageLockMode.ReadWrite, render.PixelFormat);
            b.Clear(clearColor);
            Matrix4x4.Invert(pvMat, out var invPvMat);
            int count = b.Height / renderSegment;
            Parallel.For(0, count, (index) =>
            {
                int minY = renderSegment * index;
                int maxY = Math.Min(renderSegment * (index + 1), b.Height);
                RenderPixels(b, screen, view, invPvMat, lights, 0, b.Width, minY, maxY);
            });
            Parallel.For(0, count, (index) =>
            {
                int minY = renderSegment * index;
                int maxY = Math.Min(renderSegment * (index + 1), b.Height);
                SmoothPixels(b, 0, b.Width, minY, maxY);
            });
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
