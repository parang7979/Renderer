using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine
{
    static public class ResourceManager
    {
        static private Dictionary<string, List<Mesh>> meshs = new Dictionary<string, List<Mesh>>();
        static private Dictionary<string, Texture> textures = new Dictionary<string, Texture>();

        static public void AddMesh(string path, Mesh mesh)
        {
            if (!meshs.ContainsKey(path)) meshs.Add(path, new List<Mesh> { mesh });
        }

        static public void AddMesh(string path)
        {
            var mesh = Mesh.LoadMesh(path);
            if (!meshs.ContainsKey(path)) meshs.Add(path, mesh.Values.ToList());
        }

        static public void AddTexture(string path)
        {
            if (!textures.ContainsKey(path)) textures.Add(path, new Texture(path));
        }

        static public List<Mesh> GetMesh(string path)
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
