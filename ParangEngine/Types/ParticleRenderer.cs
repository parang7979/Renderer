using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class ParticleRenderer : Renderer
    {
        public bool Pause { get; set; } = false;
        public Color Color { get; set; } = new Color(System.Drawing.Color.OrangeRed);
        public Vector3 Direction { get; set; } = -Vector3.UnitZ;

        private Random random = new Random();
        private List<Particle> particles = new List<Particle>();

        public override void Update(int delta, List<string> keys)
        {
            base.Update(delta, keys);
            if (!Pause)
            {
                var newP = new Particle(Transform.Mat, Color, Direction,
                    (float)(random.NextDouble() + 2f),
                    (float)(random.NextDouble() + 1f) * 1.25f,
                    (float)(random.NextDouble() + 1f) * 0.25f);
                particles.Add(newP);
            }
            foreach (var p in particles) 
                p.Update(delta, Transform.Mat);
            particles.RemoveAll(x => x.IsExpired);
        }

        public override void Draw(List<Camera> cameras)
        {
            Parallel.ForEach(particles.ToList(), (p) =>
            {
                p.Draw(cameras);
            });
        }
    }
}
