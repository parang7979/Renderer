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
        private List<MeshRenderer> renderers = new List<MeshRenderer>();
        private List<Texture> textures = new List<Texture>();

        public void Add(GameObject go)
        {
            objects.Add(go);
            cameras.AddRange(go.GetComponents<Camera>());
            lights.AddRange(go.GetComponents<Light>());
            var r = go.GetComponents<MeshRenderer>();
            renderers.AddRange(r);
            textures.AddRange(r.SelectMany(x => x.Material.Textures));
            textures = textures.Distinct().ToList();
        }

        public void Update()
        {
            foreach (var obj in objects) obj.Update();
        }

        public void Draw()
        {
            using (new StopWatch("Scene.Draw"))
            {
                foreach (var c in cameras) c.Lock();
                foreach (var t in textures) t.Lock();
                foreach (var r in renderers) r.Draw(cameras);
                foreach (var t in textures) t.Unlock();
                foreach (var c in cameras) c.Unlock();
            }
        }

        public void Render()
        {
            foreach (var c in cameras)
            {
                c.Render(lights);
            }
        }

        public void SwitchBuffer()
        {
            foreach (var c in cameras) c.SwitchBuffer();
        }
    }
}
