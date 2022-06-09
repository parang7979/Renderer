using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class GameObject
    {
        public Transform Transform { get; private set; }
        public Mesh Mesh { get; private set; }
        public Texture Texture { get; private set; }

        public long DrawId => (Mesh.Id << 32) + Texture.Id;
    }
}
