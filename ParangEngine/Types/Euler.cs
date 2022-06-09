using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    internal class Euler
    {
        public Vector3 Rotate 
        {
            get => rotate;
            set
            {
                rotate = value;
                Clamp();
            }
        }

        public float Pitch
        {
            get => rotate.X;
            set
            {
                rotate.X = AxisClamp(value);
            }
        }

        public float Yaw
        {
            get => rotate.Y;
            set
            {
                rotate.Y = AxisClamp(value);
            }
        }

        public float Roll
        {
            get => rotate.Z;
            set
            {
                rotate.Z = AxisClamp(value);
            }
        }

        private Vector3 rotate = Vector3.Zero;

        public Euler()
        {
        }

        public Euler(float pitch, float yaw, float roll)
        {
            rotate = new Vector3(pitch, yaw, roll);
        }

        private void Clamp()
        {
            rotate.X = AxisClamp(Rotate.X);
            rotate.Y = AxisClamp(Rotate.Y);
            rotate.Z = AxisClamp(Rotate.Z);   
        }

        private float AxisClamp(float v)
        {
            v %= 360f;
            if (v < 0f) v += 360f;
            return v;
        }

        public (Vector3 right, Vector3 up, Vector3 foward) GetLocalAxes()
        {
            var (sy, cy) = Yaw.GetSinCos();
            var (sr, cr) = Roll.GetSinCos();
            var (sp, cp) = Pitch.GetSinCos();

            return (new Vector3(cy * cr + sy * sp * sr, cp * sr, -sy * cr + cy * sp * sr),
                new Vector3(-cy * sr + sy * sp * cr, cp * cr, sy * sr + cy * sp * cr),
                new Vector3(sy * cp, -sp, cy * cp));
        }
    }
}
