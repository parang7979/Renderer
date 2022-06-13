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

        virtual public Color GetColor(Vector3 normal)
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
            var mat = Transform.Mat * pvMat;
            direction = Vector3.Normalize(Vector4.Transform(Vector4.UnitZ, mat).ToVector3());
        }

        public override Color GetColor(Vector3 normal)
        {
            var d = -Math.Min(0, Vector3.Dot(normal, direction));
            return Color * d * Intensity;
        }
    }
}
