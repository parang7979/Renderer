﻿using ParangEngine;
using ParangEngine.Types;
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

        private int fly = 0;

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
                    Color = new Color("green")
                });
                SceneManager.CurrentScene.Add(go);
                elpased = 0;
            }
        }
    }
}
