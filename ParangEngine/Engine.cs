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
                await Task.Delay(1000 / 60);
            }
        }

        private async void Render()
        {
            while (running)
            {
                renderer.Render();
                await Task.Delay(1000 / 30);
            }
        }
    }
}