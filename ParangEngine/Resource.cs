using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine
{
    internal class Resource
    {
        private Dictionary<long, Mesh> meshs = new Dictionary<long, Mesh>();
        private Dictionary<long, Texture> textures = new Dictionary<long, Texture>();
    }
}
