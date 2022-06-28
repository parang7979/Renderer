using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class Scene
    {
        public Camera MainCamera => cameras.FirstOrDefault();

        private List<GameObject> objects = new List<GameObject>();
        private List<Camera> cameras = new List<Camera>();
        private List<Light> lights = new List<Light>();
        private List<Renderer> renderers = new List<Renderer>();
        private List<Texture> textures = new List<Texture>();

        public void Add(GameObject go)
        {
            objects.Add(go);
            cameras.AddRange(go.GetComponents<Camera>());
            lights.AddRange(go.GetComponents<Light>());
            var r = go.GetComponents<Renderer>();
            renderers.AddRange(r);
            textures.AddRange(r
                .Select(x => x as MeshRenderer)
                .Where(x => x != null)
                .SelectMany(x => x.Material.Textures));
            textures = textures.Distinct().ToList();
        }

        public void Update(int delta, List<string> keys)
        {
            foreach (var obj in objects.ToList()) obj.Update(delta, keys);
        }

        public void Draw()
        {
            if (MainCamera == null) return;
            using (new StopWatch("Scene.Draw"))
            {
                var cs = cameras.ToList();
                var ts = textures.ToList();
                foreach (var c in cs) c.Lock();
                foreach (var t in ts) t.Lock();
                foreach (var r in renderers.ToList()) r.Draw(cameras);
                foreach (var t in cs) t.Unlock();
                foreach (var c in ts) c.Unlock();
            }
        }

        public void Render()
        {
            foreach (var c in cameras.ToList())
            {
                c.Render(lights.ToList());
            }
        }

        public void SwitchBuffer()
        {
            foreach (var c in cameras.ToList()) c.SwitchBuffer();
        }
    }
}
