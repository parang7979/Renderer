using ParangEngine;
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
            var d = Transform.Forward * Speed;
            pos += d;
            fly += delta;
            Transform.Position = pos;
            // if (fly > Duration) SceneManager.CurrentScene.Remove(this);
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
                go.Transform.Position = Transform.Position;
                var pos = go.Transform.Position + go.Transform.Forward;
                pos.Y -= 0.5f;
                go.Transform.Position = pos;
                go.Transform.Rotation = Transform.Rotation;
                // go.Transform.Scale = new Vector3(0.5f, 0.5f, 0.5f);
                var proj = new Projectile();
                go.AddComponent(proj);
                var material = new Material();
                material.AddTexture(Material.Type.Albedo, ResourceManager.GetTexture("UV-Texture.png"));
                material.AddTexture(Material.Type.Normal, ResourceManager.GetTexture("ChickenNorm.png"));
                material.Roughness = 0.2f;
                material.Metalic = 1f;
                var mesh = new MeshRenderer(
                    ResourceManager.GetMesh("chickenV2.obj"), material);
                go.AddComponent(mesh);
                SceneManager.CurrentScene.Add(go);
                elpased = 0;
            }
        }
    }
}
