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

        virtual public Color GetLight(Vector3 pos, Vector3 normal)
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

        public override void Update()
        {
            base.Update();
            direction = Vector3.Normalize(Vector4.Transform(new Vector4(Vector3.UnitZ, 0), Transform.Mat).ToVector3());
        }

        public override Color GetLight(Vector3 pos, Vector3 normal)
        {
            var d = Vector3.Dot(normal, direction);
            return base.GetLight(pos, normal) * (d < 0 ? -d : 0);
        }

        public override Color GetSpecular(Vector3 pos, Color albedo, Vector3 reflect)
        {
            return base.GetSpecular(pos, albedo, reflect);
        }
    }

    public class PointLight : Light
    {
        public float Radius { get; set; }

        private Vector3 center;
        private float r2;

        public PointLight(Transform transform) : base(transform)
        {

        }

        public override void Update()
        {
            base.Update();
            // 빛의 위치를 World로
            center = Vector4.Transform(new Vector4(Vector3.Zero, 1), Transform.Mat).ToVector3();
            r2 = Radius * Radius;
        }

        public override Color GetLight(Vector3 pos, Vector3 normal)
        {
            var dir = pos - center;
            var l = dir.LengthSquared();
            if (r2 < l) return Color.Black;
            var d = Vector3.Dot(normal, Vector3.Normalize(dir));
            return base.GetLight(pos, normal) * (d < 0 ? -d : 0) * (1 / l);
        }

        public override Color GetSpecular(Vector3 pos, Color albedo, Vector3 reflect)
        {
            return base.GetSpecular(pos, albedo, reflect);
        }
    }
}
