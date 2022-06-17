using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ParangEngine
{
    public class Engine
    {
        private Renderer renderer;
        private bool running;

        public Engine(Graphics g, Size res, int downScale)
        {
            renderer = new Renderer(g, res, downScale);
            running = false;
        }

        public void Start()
        {
            running = true;
            Update();
            Render();
        }

        public void Stop()
        {
            running = false;
        }

        private async void Update()
        {
            while (running)
            {
                renderer.Update();
                await Task.Delay(33);
            }
        }

        private async void Render()
        {
            while (running)
            {
                var now = DateTime.UtcNow;
                renderer.Render();
                var span = DateTime.UtcNow - now;
                var diff = (int)span.TotalMilliseconds;
                if (diff < 33)
                    await Task.Delay(33 - diff);
                else
                    await Task.Delay(1);
            }
        }
    }
}