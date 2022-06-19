using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class MeshRenderer : Component
    {
        public long Id => material?.Id ?? -1;
        public Material Material => material;

        private Mesh mesh;
        private Material material;

        public MeshRenderer(Mesh mesh, Material material)
        {
            this.mesh = mesh;
            this.material = material;
        }

        private void UpdateNormal()
        {

        }

        public void Draw(List<Camera> cameras)
        {
            foreach(var c in cameras)
            {
                if (!c.DrawCheck(Transform)) continue;
                c.DrawMesh(Transform, mesh, material);
            }
        }
    }
}
