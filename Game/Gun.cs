using ParangEngine;
using ParangEngine.Types;
using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class Projectile : Component
    {
        public float Speed { get; set; } = 1f;
        public int Duration { get; set; } = 2000;

        protected int fly = 0;

        public override void Update(int delta, List<string> keys)
        {
            var rot = VectorExtension.RotateTo(Transform.Position, Vector3.Zero);

            var pos = Transform.Position;
            pos += Transform.Forward * Speed;
            Transform.Position = pos;
            fly += delta;
            if (fly > Duration) 
                SceneManager.CurrentScene.Remove(GameObject);
        }
    }

    internal class CurveProjectile : Projectile
    {
        public Vector3 StartRotate { get; set; } = Vector3.Zero;

        public override void Update(int delta, List<string> keys)
        {
            var pos = Transform.Position;
            pos += Transform.Forward * Speed;
            Transform.Position = pos;
            fly += delta;
            if (fly > Duration)
                SceneManager.CurrentScene.Remove(GameObject);
        }
    }

    internal class Gun : Component
    {
        private int elpased = 0;

        public override void Update(int delta, List<string> keys)
        {
            elpased += delta;
            if (elpased > 500 && keys.Contains("Space"))
            {
                var go = new GameObject();
                go.Transform.Position = Vector3.Transform(Transform.Position, Transform.Mat);
                go.Transform.Rotation = Transform.Rotation;
                go.AddComponent(new Projectile());
                go.AddComponent(new ParticleRenderer
                {
                    Color = new Color("green"),
                    Velocity = new Vector2(0f, 0.5f),
                    Power = new Vector2(4f, 3f),
                    Duration = new Vector2(0f, 1f)
                });
                go.AddComponent(new PointLight
                {
                    Color = new Color("green"),
                    Intensity = 2f,
                    Radius = 2f,
                });
                SceneManager.CurrentScene.Add(go);
                elpased = 0;
            }
        }
    }
}
