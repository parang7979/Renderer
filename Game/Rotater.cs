using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class Rotater : Component
    {
        public override void Update(int delta, List<string> keys)
        {
            base.Update(delta, keys);
            var rot = Transform.Rotation;
            rot.Y += 2;
            Transform.Rotation = rot;
        }
    }

    internal class MatalicController : Component
    {
        public override void Update(int delta, List<string> keys)
        {
            base.Update(delta, keys);
            var r = GameObject.GetComponent<MeshRenderer>();
            r.Material.Metalic = (r.Material.Metalic + 0.01f) % 1f;
        }
    }

    internal class RoughnessController : Component
    {
        public override void Update(int delta, List<string> keys)
        {
            base.Update(delta, keys);
            var r = GameObject.GetComponent<MeshRenderer>();
            r.Material.Roughness = (r.Material.Roughness + 0.01f) % 1f;
        }
    }

    internal class JetEngine : Component
    {
        public Color Color { get; set; } = Color.White;

        private ParticleRenderer renderer;
        private PointLight light;

        public override void Setup()
        {
            renderer = new ParticleRenderer()
            {
                Color = Color,
            };
            GameObject.AddComponent(renderer);
            light = new PointLight()
            {
                Color = Color,
                Intensity = 1f,
                Radius = 2f,
            };
            GameObject.AddComponent(light);
        }

        public override void Update(int delta, List<string> keys)
        {
            var intensity = 0.5f;
            if (keys.Contains("Down"))
            {
                renderer.Pause = true;
            }
            else
            {
                renderer.Pause = false;
                intensity += 0.5f;
            }

            if (keys.Contains("Up"))
            {
                intensity += 1f;
            }


            light.Intensity = intensity;
            base.Update(delta, keys);
        }
    }

    internal class Input : Component
    {
        public override void Update(int delta, List<string> keys)
        {
            base.Update(delta, keys);
            var pos = Transform.Position;
            var rot = Transform.Rotation;

            var d = delta / 1000f;
            if (keys.Contains("Up"))
                pos += Vector3.UnitZ * d * 4;

            if (keys.Contains("Down"))
                pos -= Vector3.UnitZ * d * 4;

            if (keys.Contains("Left"))
            {
                pos -= Vector3.UnitX * d * 4;
                rot.Z += d * 180;
            }

            if (keys.Contains("Right"))
            {
                pos += Vector3.UnitX * d * 4;
                rot.Z -= d * 180;
            }

            rot.Z = Math.Min(rot.Z, 45f);
            rot.Z = Math.Max(rot.Z, -45f);
            if (rot.Z > 5f) rot.Z -= d * 90;
            else if (rot.Z < -5f) rot.Z += d * 90;
            else rot.Z = 0f;

            Transform.Position = pos;
            Transform.Rotation = rot;
        }
    }
}
