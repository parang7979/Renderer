using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    internal class Particle
    {
        public bool IsExpired => power <= 0.1f;

        private Vector3 position;
        private Vector3 direction;
        private Color color;
        private float power;
        private float delta;

        public Particle (Matrix4x4 mat, Color color, Vector3 dir, float v, float p, float t)
        {
            position = Vector3.Transform(Vector3.Zero, mat);
            this.color = color;
            direction = Vector3.TransformNormal(dir, mat) * v;
            power = p;
            delta = p / t;
        }

        public void Update(int delta, Matrix4x4 mat)
        {
            var d = (delta / 1000f);
            position += direction * d;
            power -= this.delta * d;
        }

        public void Draw(List<Camera> cameras)
        {
            Parallel.ForEach(cameras, (c) =>
            {
                c.DrawParticle(position, color * (1 + power));
            });
        }
    }
}
