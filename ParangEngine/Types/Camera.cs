using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class Camera
    {
        public Transform Transform { get; set; } = new Transform();

        static public Vertex operator *(Camera c, Vertex v)
        {
            var mat = Matrix4x4.CreateLookAt(c.Transform.Position, c.Transform.Position + c.Transform.Forward, c.Transform.Up);
            v.Pos = Vector4.Transform(v.Pos, mat);
            return v;
        }
    }
}
