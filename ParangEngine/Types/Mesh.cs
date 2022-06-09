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

        public void Render(Camera camera, Transform transform, Texture texture)
        {
            int count = indicies.Count / 3 * 3;
            for (int i = 0; i < count; i += 3)
            {
                // 정점 변환 (VS)
                var v1 = vertices[indicies[i]] * transform;
                var v2 = vertices[indicies[i + 1]] * transform;
                var v3 = vertices[indicies[i + 2]] * transform;
                camera.Render(v1, v2, v3, texture);
            }
            camera.RenderAxes(transform);
        }
    }
}
