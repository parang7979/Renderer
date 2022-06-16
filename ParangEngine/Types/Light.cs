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

        virtual public Color GetColor(Vector4 pos, Vector3 normal)
        {
            return Color * Intensity;
        }

        virtual public Color GetSpecular(Vector3 pos, Color albedo, Vector3 reflect)
        {
            return Color * albedo * Intensity;
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

        public override Color GetColor(Vector4 pos, Vector3 normal)
        {
            var d = -Math.Min(0, Vector3.Dot(normal, direction));
            return base.GetColor(pos, normal) * d;
        }

        public override Color GetSpecular(Vector3 pos, Color albedo, Vector3 reflect)
        {
            var d = -Math.Min(0, Vector3.Dot(reflect, direction));
            return base.GetSpecular(pos, albedo, reflect) * d;
        }
    }

    public class PointLight : Light
    {
        public float Radius { get; set; }

        private Matrix4x4 invMat;

        public PointLight(Transform transform) : base(transform)
        {

        }

        public override void Setup(Matrix4x4 pvMat)
        {
            base.Setup(pvMat);
            Matrix4x4.Invert(pvMat, out invMat);
        }

        public override Color GetColor(Vector4 pos, Vector3 normal)
        {
            // 위치를 빛의 좌표계로 이동함
            pos = pos.ToInvNDC();
            var p = Vector4.Transform(pos, invMat).ToVector3();
            var c = Vector4.Transform(new Vector4(Vector3.Zero, 1), Transform.Mat).ToVector3();
            var dir = p - c;
            // 벡터가 곧 방향과 길이
            var l = dir.Length();
            if (Radius < l) return Color.Black;
            var d = Vector3.Dot(normal, Vector3.Normalize(dir)); // > 0f ? 1f : 0f;
            // var d = -Math.Min(0, Vector3.Dot(normal, dir));
            return base.GetColor(pos, normal) * (d < 0 ? -d : 0) * (1 / (l * l));//(1 - r / Radius);
        }

        public override Color GetSpecular(Vector3 pos, Color albedo, Vector3 reflect)
        {
            /* var dir = pos - position;
            var l = dir.Length();
            if (radius < l) return Color.Black;
            var d = -Math.Min(0, Vector3.Dot(reflect, dir));
            return base.GetSpecular(pos, albedo, reflect) * d * (1 / (l * l)); */
            return base.GetSpecular(pos, albedo, reflect);
        }
    }
}
