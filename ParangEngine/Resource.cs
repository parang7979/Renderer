using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine
{
    static public class Resource
    {
        static private Dictionary<string, Mesh> meshs = new Dictionary<string, Mesh>();
        static private Dictionary<string, Texture> textures = new Dictionary<string, Texture>();

        static public void AddMesh(string path, Mesh mesh)
        {
            if (!meshs.ContainsKey(path)) meshs.Add(path, mesh);
        }

        static public void AddMesh(string path)
        {
            if (!meshs.ContainsKey(path)) meshs.Add(path, new Mesh(path));
        }

        static public void AddTexture(string path)
        {
            if (!textures.ContainsKey(path)) textures.Add(path, new Texture(path));
        }

        static public Mesh GetMesh(string path)
        {
            if (meshs.ContainsKey(path)) return meshs[path];
            return null;
        }

        static public Texture GetTexture(string path)
        {
            if (textures.ContainsKey(path)) return textures[path];
            return null;
        }
    }
}
