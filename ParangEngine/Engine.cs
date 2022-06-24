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

        private Scene scene => SceneManager.CurrentScene;
        private bool running;

        private List<string> keyDowns = new List<string>();
        private List<string> keyUps = new List<string>();

        public Engine(Graphics graphics, Size resolution)
        {
            this.resolution = resolution;
            this.graphics = graphics;
            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(resolution.Width + 1, resolution.Height + 1);
            buffer = context.Allocate(graphics, new Rectangle(0, 0, resolution.Width, resolution.Height));
            Gizmos.CreateGrid(10);
            running = false;
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
                    scene.Update((int)span.TotalMilliseconds, keyDowns.Distinct().ToList());
                    keyDowns.Clear();
                }
                now = n;
                await Task.Delay(33);
            }
        }

        public void KeyDown(string key)
        {
            keyDowns.Add(key);
        }

        public void KeyUp(string key)
        {
            keyUps.Add(key);
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
            buffer.Graphics.DrawImage(scene.MainCamera.GetBuffer(GBuffer.BufferType.Albedo), 0, 0, resolution.Width / 2, resolution.Height / 2);
            buffer.Graphics.DrawImage(scene.MainCamera.GetBuffer(GBuffer.BufferType.Position), resolution.Width / 2, 0, resolution.Width / 2, resolution.Height / 2);
            buffer.Graphics.DrawImage(scene.MainCamera.GetBuffer(GBuffer.BufferType.Normal), 0, resolution.Height / 2, resolution.Width / 2, resolution.Height / 2);
            buffer.Graphics.DrawImage(scene.MainCamera.GetBuffer(GBuffer.BufferType.Surface), resolution.Width / 2, resolution.Height / 2, resolution.Width / 2, resolution.Height / 2);
        }

        private void RenderTask()
        {
            using (new StopWatch("Engine.RenderTask"))
            {
                if (scene.MainCamera == null) return;
                scene.Render();
                buffer.Graphics.DrawImage(scene.MainCamera.RenderTarget, 0, 0, resolution.Width, resolution.Height);
                // RenderDebug();
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
                if (diff < 33) await Task.Delay(33 - diff);
            }
        }
    }
}