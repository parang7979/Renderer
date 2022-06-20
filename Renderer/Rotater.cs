using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderer
{
    internal class Rotater : Component
    {
        public override void Update()
        {
            base.Update();
            var rot = Transform.Rotation;
            rot.Y += 2;
            Transform.Rotation = rot;
        }
    }
}
