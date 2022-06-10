using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class Mesh
    {
        public int Id { get; set; }
        
        private List<Vertex> vertices;
        private List<int> indicies;

        public Mesh(int id, List<Vertex> vertices, List<int> indicies)
        {
            this.Id = id;
            this.vertices = vertices;
            this.indicies = indicies;
        }

        public void Render(in Camera camera, in Transform transform, in Texture texture)
        {
            // 애니메이션, 스키닝, 등등..
            camera.Render(vertices, 
                transform, texture,
                (v, m) => Vertex.Transform(v, m));
            camera.RenderAxes(transform);
        }
    }
}
