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
        public Color Color { get; set; } = Color.White;

        private Random random = new Random();
        private List<Vector3> particles = new List<Vector3>();

        public override void Update(int delta, List<string> keys)
        {
            base.Update(delta, keys);
            var p = new Vector3(
                (float)random.NextDouble() * 0.5f,
                (float)random.NextDouble() * 0.5f,
                (float)random.NextDouble() * 0.5f);
            particles.Add(Vector3.Transform(p, Transform.Mat));
            if (particles.Count > 20)
                particles.RemoveAt(0);
        }

        public override void Draw(List<Camera> cameras)
        {
            Parallel.ForEach(cameras, (c) =>
                {
                    var rnd = new Random();
                    foreach(var p in particles.ToList())
                    {
                        c.DrawParticle(p, Color * 30);
                    }
                });
        }
    }
}
