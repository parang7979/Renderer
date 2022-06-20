using ParangEngine.Types;
using ParangEngine.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ParangEngine
{
    public class Engine
    {
        private Scene scene;

        private Size resolution;
        private Graphics graphics;
        private BufferedGraphicsContext context;
        private BufferedGraphics buffer;

        private bool running;

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

        public void SetScene(Scene scene)
        {
            this.scene = scene;
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
        }

        private async void Update()
        {
            while (running)
            {
                if (scene != null)
                    scene.Update();
                await Task.Delay(33);
            }
        }

        private void DrawTask()
        {
            if (scene.MainCamera == null) return;
            scene.Draw();
        }

        private void RenderDebug()
        {
            buffer.Graphics.DrawImage(scene.MainCamera.RenderTarget, 0, 0, resolution.Width / 2, resolution.Height / 2);
            buffer.Graphics.DrawImage(scene.MainCamera.GetBuffer(GBuffer.BufferType.Albedo), resolution.Width / 2, 0, resolution.Width / 2, resolution.Height / 2);
            buffer.Graphics.DrawImage(scene.MainCamera.GetBuffer(GBuffer.BufferType.Position), 0, resolution.Height / 2, resolution.Width / 2, resolution.Height / 2);
            buffer.Graphics.DrawImage(scene.MainCamera.GetBuffer(GBuffer.BufferType.Normal), resolution.Width / 2, resolution.Height / 2, resolution.Width / 2, resolution.Height / 2);
        }

        private void RenderTask()
        {
            if (scene.MainCamera == null) return;
            scene.Render();
            buffer.Graphics.DrawImage(scene.MainCamera.RenderTarget, 0, 0, resolution.Width, resolution.Height);
            // RenderDebug();
            buffer.Render(graphics);
        }

        private async void Render()
        {
            Task task1;
            Task task2;
            while (running)
            {
                var now = DateTime.UtcNow.Ticks;
                if (scene != null)
                {
                    scene.SwitchBuffer();
                    task1 = Task.Factory.StartNew(DrawTask);
                    task2 = Task.Factory.StartNew(RenderTask);
                    while (!(task1.IsCompleted && task2.IsCompleted)) ;
                }
                var span = DateTime.UtcNow.Ticks - now;
                var diff = (int)(span / TimeSpan.TicksPerMillisecond);
                if (diff < 33) await Task.Delay(33 - diff);
            }
        }
    }
}