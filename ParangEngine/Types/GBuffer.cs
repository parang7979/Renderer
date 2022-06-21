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

        private byte multiDraw = 24;
        private byte multiRender = 8;
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
            drawPixelPool = new List<DrawPixelsArg>();
            for (int i = 0; i < multiDraw * multiDraw; i++)
                drawPixelPool.Add(new DrawPixelsArg());
            renderPixelPool = new List<RenderPixelsArg>();
            for (int i = 0; i < multiRender * multiRender; i++)
                renderPixelPool.Add(new RenderPixelsArg());
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

        private void DrawPixel(DrawPixelsArg data, int x, int y)
        {
            Point p = new Point(x, y);
            Vector2 w = p.ToVector2(data.Screen) - data.V1.Vector2;
            float wDu = Vector2.Dot(w, data.U);
            float wDv = Vector2.Dot(w, data.V);
            float s = (wDv * data.Dots.X - wDu * data.Dots.Y) * data.Dots.W;
            float t = (wDu * data.Dots.X - wDv * data.Dots.Z) * data.Dots.W;
            float o = 1f - s - t;
            if ((0f <= s && s <= 1f) && (0f <= t && t <= 1f) && (0f <= o && o <= 1f))
            {
                // position
                var pos = data.V1.View * o + data.V2.View * s + data.V3.View * t;
                // output buffer and depth testing
                if (!SetPositionBuffer(data.Screen, x, y, pos)) return;

                var z = data.InvZs.X * o + data.InvZs.Y * s + data.InvZs.Z * t;
                var invZ = 1f / z;

                // uvs
                var uvs = new List<Vector2>();
                for (int i = 0; i < (int)Material.Type.Max; i++)
                    uvs.Add((data.V1.UVs[i] * o * data.InvZs.X + data.V2.UVs[i] * s * data.InvZs.Y + data.V3.UVs[i] * t * data.InvZs.Z) * invZ);

                // normal
                var normal = data.V1.Normal * o + data.V2.Normal * s + data.V3.Normal * t;
                var rotNormal = data.V1.RotNormal * o + data.V2.RotNormal * s + data.V3.RotNormal * t;

                // vertex color
                Color vertexColor = (data.V1.Color * o * data.InvZs.X + data.V2.Color * s * data.InvZs.Y + data.V3.Color * t * data.InvZs.Z) * invZ;

                // convert ps
                var output = data.Material.Convert(uvs, normal, rotNormal, vertexColor);

                // output buffer
                SetNormalBuffer(x, y, Vector3.Normalize(output.Normal));
                SetAlbedoBuffer(x, y, output.Color);
                SetSurfaceBuffer(x, y, output.Surface);
            }
        }

        private void DrawPixels(object arg)
        {
            var data = (DrawPixelsArg)arg;
            for (int x = data.MinX; x < data.MaxX; x++)
            {
                for (int y = data.MinY; y < data.MaxY; y++)
                {
                    DrawPixel(data, x, y);
                }
            }
        }

        public void DrawTriangle(Screen screen, OutputVS v1, OutputVS v2, OutputVS v3, Material material)
        {
            if (locks.Count == 0) return;

            var p1 = screen.ToPoint(v1.Position);
            var p2 = screen.ToPoint(v2.Position);
            var p3 = screen.ToPoint(v3.Position);

            var min = screen.Clamp(new Point(
                MathExtension.Min3(p1.X, p2.X, p3.X),
                MathExtension.Min3(p1.Y, p2.Y, p3.Y))); // 좌상

            var max = screen.Clamp(new Point(
                MathExtension.Max3(p1.X, p2.X, p3.X),
                MathExtension.Max3(p1.Y, p2.Y, p3.Y))); // 우하

            var u = v2.Vector2 - v1.Vector2;
            var v = v3.Vector2 - v1.Vector2;
            Vector4 dots = 
                new Vector4(Vector2.Dot(u, v), Vector2.Dot(v, v), Vector2.Dot(u, u), 0);
            var d = dots.X * dots.X - dots.Y * dots.Z;
            if (d == 0f) return;
            dots.W = 1 / d;
            Vector3 invZs = new Vector3(1f / v1.W, 1f / v2.W, 1f / v3.W);
            int w = (max.X - min.X) / multiDraw;
            int h = (max.Y - min.Y) / multiDraw;
            int wr = (max.X - min.X) % multiDraw;
            int hr = (max.Y - min.Y) % multiDraw;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < multiDraw; i++)
            {
                for (int j = 0; j < multiDraw; j++)
                {
                    var arg = drawPixelPool[i * multiDraw + j];
                    arg.Setup(screen, v1, v2, v3, material, u, v, dots, invZs,
                        min.X + i * w, min.X + (i + 1) * w,
                        min.Y + j * h, min.Y + (j + 1) * h);
                    if (i == multiDraw - 1) arg.MaxX += wr;
                    if (j == multiDraw - 1) arg.MaxY += hr;
                    tasks.Add(Task.Factory.StartNew(DrawPixels, arg));
                }
            }
            while (tasks.Any(x => !x.IsCompleted)) ;
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

        private void RenderPixel(RenderPixelsArg data, int x, int y)
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
                    p.A * data.Screen.ViewDistance);

                // 위치를 월드 좌표계로 이동함
                var invPos = Vector4.Transform(pos.ToInvNDC(), data.InvPvMat);
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
                    foreach (var light in data.Lights)
                        l += light.GetLight(invPos.ToVector3(), data.View, normal, surface);
                    a *= l;
                }
            }
            data.Bitmap.SetPixel(x, y, a);
        }

        private void RenderPixels(object arg)
        {
            var data = (RenderPixelsArg)arg;
            for (int x = data.MinX; x < data.MaxX; x++)
            {
                for (int y = data.MinY; y < data.MaxY; y++)
                {
                    RenderPixel(data, x, y);
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
            int w = b.Width / multiRender;
            int h = b.Height / multiRender;
            int wr = b.Width % multiRender;
            int hr = b.Height % multiRender;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < multiRender; i++)
            {
                for (int j = 0; j < multiRender; j++)
                {
                    var arg = renderPixelPool[i * multiRender + j];
                    arg.Setup(b, screen, view, invPvMat, lights,
                        i * w, (i + 1) * w, j * h, (j + 1) * h);
                    if (i == multiRender - 1) arg.MaxX += wr;
                    if (j == multiRender - 1) arg.MaxY += hr;
                    tasks.Add(Task.Factory.StartNew(RenderPixels, arg));
                }
            }
            while (tasks.Any(x => !x.IsCompleted)) ;
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
