using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class Light : Component
    {
        public Color Color { get; set; }
        public float Intensity { get; set; } = 1f;

        public Light(Transform transform) : base(transform)
        {

        }

        virtual public void Setup(Matrix4x4 pvMat)
        {

        }

        virtual public Color GetColor(Vector3 pos, Vector3 normal)
        {
            return Color * Intensity;
        }
    }

    public class DirectionalLight : Light
    {
        private Vector3 direction = Vector3.UnitZ;

        public DirectionalLight(Transform transform) : base(transform)
        {

        }

        public override void Setup(Matrix4x4 pvMat)
        {
            base.Setup(pvMat);
            var mat = Transform.Mat * pvMat;
            direction = Vector3.Normalize(Vector4.Transform(Vector4.UnitZ, mat).ToVector3());
        }

        public override Color GetColor(Vector3 pos, Vector3 normal)
        {
            var d = -Math.Min(0, Vector3.Dot(normal, direction));
            return base.GetColor(pos, normal) * d;
        }
    }

    public class PointLight : Light
    {
        public float Radius { get; set; }

        private Vector3 position = Vector3.Zero;

        public PointLight(Transform transform) : base(transform)
        {

        }

        public override void Setup(Matrix4x4 pvMat)
        {
            base.Setup(pvMat);
            var mat = Transform.Mat * pvMat;
            position = Vector4.Transform(new Vector4(Vector3.Zero, 1), mat).ToVector3();
        }

        public override Color GetColor(Vector3 pos, Vector3 normal)
        {
            var dir = pos - position;
            var r = dir.Length();
            if (Radius < r) return Color.Black;
            var d = Math.Min(0f, Vector3.Dot(normal, dir)) < 0f ? 1f : 0f;
            return base.GetColor(pos, normal) * d * (1 - r / Radius);
        }
    }
}
