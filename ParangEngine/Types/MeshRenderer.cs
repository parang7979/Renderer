using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class MeshRenderer : Renderer
    {
        public Material Material => material;

        private List<Mesh> meshs;
        private Material material;

        public MeshRenderer(List<Mesh> meshs, Material material)
        {
            this.meshs = meshs;
            this.material = material;
        }

        public override void Draw(List<Camera> cameras)
        {
            if (meshs == null) return;
            Parallel.ForEach(cameras, (c) =>
                {
                    // if (!c.DrawCheck(Transform)) continue;
                    foreach(var mesh in meshs)
                        c.DrawMesh(Transform, mesh, material);
                });
        }
    }
}
