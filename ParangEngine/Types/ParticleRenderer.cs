using ParangEngine.Utils;
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
        public Vector3 AngleMin { get; set; } = Vector3.Zero;
        public Vector3 AngleMax { get; set; } = Vector3.Zero;
        public Vector2 Velocity { get; set; } = Vector2.One;
        public Vector2 Power { get; set; } = Vector2.One;
        public Vector2 Duration { get; set; } = Vector2.One;

        private Random random = new Random();
        private List<Particle> particles = new List<Particle>();

        public override void Update(int delta, List<string> keys)
        {
            base.Update(delta, keys);
            if (!Pause)
            {
                var q = Quaternion.CreateFromYawPitchRoll(
                    ((float)random.NextDouble() * (AngleMax.Y - AngleMin.Y) + AngleMin.Y).ToRad(),
                    ((float)random.NextDouble() * (AngleMax.X - AngleMin.X) + AngleMin.X).ToRad(),
                    ((float)random.NextDouble() * (AngleMax.Z - AngleMin.Z) + AngleMin.Z).ToRad());
                
                var newP = new Particle(Transform.Mat, Color, 
                    Vector3.Transform(Direction, q),
                    (float)random.NextDouble() * (Velocity.Y - Velocity.X) + Velocity.X,
                    (float)random.NextDouble() * (Power.Y - Power.X) + Power.X,
                    (float)random.NextDouble() * (Duration.Y - Duration.X) + Duration.X);
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
