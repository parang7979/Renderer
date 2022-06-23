using System.Collections.Generic;
using System.Numerics;

namespace ParangEngine.Types
{
    public partial class Camera : Component
    {
        public Screen Screen { get; private set; }
        public float Fov { get; set; } = 60f;
        public Color ClearColor { get; set; } = Color.Black;

        private int current = 0;
        private GBuffer[] gBuffers = new GBuffer[2];
        
        private Matrix4x4 vMat;
        private Matrix4x4 pMat;
        private Matrix4x4 pvMat;
        private Frustum frustum;

        public Camera(int width, int height, float fov)
        {
            Screen = new Screen(width, height);
            Fov = fov;
            gBuffers[0] = new GBuffer(width, height);
            gBuffers[1] = new GBuffer(width, height);
        }

        public void SwitchBuffer()
        {
            current = (current + 1) % 2;
        }

        public override void Update(int delta, List<string> keys)
        {
            base.Update(delta, keys);
            var pos = Transform.Position;
            var d = delta / 1000f;
            if (keys.Contains("W"))
                pos += Transform.Forward * d * 0.01f;

            if (keys.Contains("S"))
                pos -= Transform.Forward * d * 0.01f;

            if (keys.Contains("A"))
                pos -= Transform.Right * d * 0.01f;

            if (keys.Contains("D"))
                pos += Transform.Right * d * 0.01f;

            Transform.Position = pos;
        }
    }
}
