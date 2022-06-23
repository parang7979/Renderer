using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
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
}
