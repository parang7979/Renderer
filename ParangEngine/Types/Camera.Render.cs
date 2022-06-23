using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public partial class Camera
    {
        public Image RenderTarget => renderBuffer.RenderTarget;

        private GBuffer renderBuffer => gBuffers[(current + 1) % 2];

        public void Render(List<Light> lights)
        {
            renderBuffer.Lock(false, ClearColor);
            if (!renderBuffer.IsLock) return;
            renderBuffer.Render(Screen, Transform.Forward, pvMat, lights);
            renderBuffer.Unlock();
        }

        public Image GetBuffer(GBuffer.BufferType type)
        {
            return renderBuffer.GetBuffer(type);
        }
    }
}
