using ParangEngine.Types;
using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace ParangEngine
{
    public class Engine
    {
        private Size resolution;
        private Graphics graphics;
        private BufferedGraphicsContext context;
        private BufferedGraphics buffer;
        private Font font;
        private Brush brush;
        private bool running;
        private bool showDebug;
        
        private int targetFps;
        private int avrFps;
        private int targetFramePerMs;
        private int avrFramePerMs;

        private List<string> keys = new List<string>();

        private Scene scene => SceneManager.CurrentScene;

        public Engine(Graphics graphics, Size resolution, int targetFps)
        {
            this.resolution = resolution;
            this.graphics = graphics;
            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(resolution.Width + 1, resolution.Height + 1);
            buffer = context.Allocate(graphics, new Rectangle(0, 0, resolution.Width, resolution.Height));
            font = new Font("Arial", 16);
            brush = new SolidBrush(System.Drawing.Color.White);
            Gizmos.CreateGrid(10);
            running = false;
            showDebug = false;
            this.targetFps = targetFps;
            targetFramePerMs = 1000 / targetFps;
        }

        public void Start()
        {
            running = true;
            Task.Factory.StartNew(Update);
            Task.Factory.StartNew(Render);
        }

        public void Stop()
        {
            running = false;
            Console.WriteLine("----AVR----");
            foreach (var a in StopWatch.Avrs)
                Console.WriteLine($"[AVR] {a.Key} : {a.Value}ms");
        }

        private async void Update()
        {
            var now = DateTime.UtcNow;
            while (running)
            {
                var n = DateTime.UtcNow;
                var span = n - now;
                if (scene != null)
                {
                    scene.Update((int)span.TotalMilliseconds, keys);
                }
                now = n;
                await Task.Delay(targetFramePerMs);
            }
        }

        public void KeyDown(string key)
        {
            if (key == "F1") showDebug = !showDebug;

            if (!keys.Contains(key))
                keys.Add(key);
        }

        public void KeyUp(string key)
        {
            if (keys.Contains(key))
                keys.Remove(key);
        }

        private void DrawTask()
        {
            using(new StopWatch("Engine.DrawTask"))
            {
                scene.Draw();
            }
        }

        private void RenderDebug()
        {
            buffer.Graphics.DrawImage(scene.MainCamera.GetBuffer(GBuffer.BufferType.Albedo), 0, 0, resolution.Width / 5, resolution.Height / 5);
            buffer.Graphics.DrawImage(scene.MainCamera.GetBuffer(GBuffer.BufferType.Position), 0, resolution.Height / 5, resolution.Width / 5, resolution.Height / 5);
            buffer.Graphics.DrawImage(scene.MainCamera.GetBuffer(GBuffer.BufferType.Normal), 0, resolution.Height / 5 * 2, resolution.Width / 5, resolution.Height / 5);
            buffer.Graphics.DrawImage(scene.MainCamera.GetBuffer(GBuffer.BufferType.Surface), 0, resolution.Height / 5 * 3, resolution.Width / 5, resolution.Height / 5);
        }

        private void RenderTask()
        {
            using (new StopWatch("Engine.RenderTask"))
            {
                if (scene.MainCamera == null) return;
                scene.Render();
                buffer.Graphics.DrawImage(scene.MainCamera.RenderTarget, 0, 0, resolution.Width, resolution.Height);
                if (showDebug) RenderDebug();
                DrawText($"{avrFramePerMs}ms [{targetFramePerMs}]", 5f, 10f);
                DrawText($"FPS : {avrFps} [{targetFps}]", 5f, 30f);
                buffer.Render(graphics);
            }
        }

        private async void Render()
        {
            while (running)
            {
                var now = DateTime.UtcNow.Ticks;
                if (scene != null)
                {
                    scene.SwitchBuffer();
                    Parallel.For(0, 2, (i) =>
                    {
                        if (i == 0) DrawTask();
                        else RenderTask();
                    });
                }
                var span = DateTime.UtcNow.Ticks - now;
                var diff = (int)(span / TimeSpan.TicksPerMillisecond);
                if (diff < targetFramePerMs)
                {
                    avrFramePerMs = targetFramePerMs;// (avrFramePerMs + targetFramePerMs) / 2;
                    avrFps = targetFps;// (avrFps + 1000 / targetFramePerMs) / 2;
                    await Task.Delay(targetFramePerMs);
                }
                else
                {
                    avrFramePerMs = diff;// (avrFramePerMs + diff) / 2;
                    avrFps = 1000 / diff;// (avrFps + 1000 / diff) / 2;
                }
            }
        }

        public void DrawText(string text, float x, float y)
        {
            buffer.Graphics.DrawString(text, font, brush, x, y);
        }
    }
}