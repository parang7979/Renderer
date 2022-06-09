using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ParangEngine.Utils;

namespace ParangEngine.Types
{
    public class Transform
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation
        {
            get => euler;
            set
            {
                euler = value;
                quaternion = Quaternion.CreateFromYawPitchRoll(euler.Y.ToRad(), euler.X.ToRad(), euler.Z.ToRad());
            }
        }
        public Vector3 Scale { get; set; } = Vector3.One;

        public Vector3 Right => Vector3.Transform(Vector3.UnitX, quaternion);
        public Vector3 Up => Vector3.Transform(Vector3.UnitY, quaternion);
        public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, quaternion);

        private Vector3 euler = Vector3.Zero;
        private Quaternion quaternion = Quaternion.Identity;

        static public Vertex operator *(Transform t, Vertex v)
        {
            v.Pos = Vector4.Transform(v.Pos, t.quaternion);
            var mat = Matrix4x4.CreateTranslation(t.Position) * Matrix4x4.CreateScale(t.Scale);
            v.Pos = Vector4.Transform(v.Pos, mat);
            return v;
        }
    }
}
