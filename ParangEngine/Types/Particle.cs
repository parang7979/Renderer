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
        public Color Color { get; set; }
        public Vector3 Direction { get; set; }
        public float Velocity { get; set; }
        public float Power { get; set; }
        public float LifeTime { get; set; }

        public bool IsExpired => Power <= 0.1f;

        private Vector3 position;

        public Particle (Matrix4x4 mat)
        {
            position = Vector3.Transform(Vector3.Zero, mat);
        }

        public void Update(int delta, Matrix4x4 mat)
        {
            var d = (delta / 1000f);
            position += Vector3.TransformNormal(Direction, mat) * d * Velocity;
            Power -= (Power / LifeTime) * d;
        }

        public void Draw(List<Camera> cameras)
        {
            Parallel.ForEach(cameras, (c) =>
            {
                c.DrawParticle(position, Color * (1 + Power));
            });
        }
    }
}
