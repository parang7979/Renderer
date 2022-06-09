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
        public Vector3 Rotation { get; set; } = Vector3.Zero;
        public Vector3 Scale { get; set; } = Vector3.One;

        public Vector3 Right => Vector3.Transform(Vector3.UnitX, quaternion);
        public Vector3 Up => Vector3.Transform(Vector3.UnitY, quaternion);
        public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, quaternion);

        private Quaternion quaternion = Quaternion.Identity;
        private Matrix4x4 mat;

        public void Update()
        {
            quaternion = Quaternion.CreateFromYawPitchRoll(Rotation.Y.ToRad(), Rotation.X.ToRad(), Rotation.Z.ToRad());
            mat = Matrix4x4.CreateTranslation(Position) * Matrix4x4.CreateScale(Scale);
        }

        static public Vertex operator *(Vertex v, Transform t)
        {
            v.Pos *= t;
            return v;
        }

        static public Vector4 operator *(Vector4 pos, Transform t)
        {
            pos = Vector4.Transform(pos, t.quaternion);
            pos = Vector4.Transform(pos, t.mat);
            return pos;
        }
    }
}
