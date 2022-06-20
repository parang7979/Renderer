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
        private Dictionary<Material, List<MeshRenderer>> renderers = new Dictionary<Material, List<MeshRenderer>>();

        public void Add(GameObject go)
        {
            objects.Add(go);
            var camera = go.GetComponent<Camera>();
            if (camera != null) cameras.Add(camera);
            var light = go.GetComponent<Light>();
            if (light != null) lights.Add(light);
            var renderer = go.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                if (renderers.ContainsKey(renderer.Material))
                {
                    renderers[renderer.Material].Add(renderer);
                }
                else
                {
                    var r = new List<MeshRenderer>();
                    r.Add(renderer);
                    renderers.Add(renderer.Material, r);
                }
            }
        }

        public void Update()
        {
            foreach (var obj in objects) obj.Update();
        }

        public void Draw()
        {
            foreach (var c in cameras) c.Lock();
            foreach(var r in renderers)
            {
                var mat = r.Key;
                if (mat != null) mat.Lock();
                foreach(var m in r.Value)
                {
                    m.Draw(cameras);
                }
                if (mat != null) mat.Unlock();
            }
            foreach (var c in cameras) c.Unlock();
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
