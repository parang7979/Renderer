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

        virtual public Color GetLight(Vector3 pos, Vector3 view, Vector3 normal, Color surface)
        {
            return Color * Intensity;
        }

        virtual public float GetSpecular(Vector3 dir, Vector3 view, Vector3 normal, Color surface)
        {
            var reflect = Vector3.Reflect(dir, normal);
            var d = Vector3.Dot(reflect, view);
            return ((d < 0 ? -d : 0) * surface.R) * (1f - surface.G);
        }
    }

    public class DirectionalLight : Light
    {
        public float Ambient { get; set; } = 0.05f;

        private Vector3 direction = Vector3.UnitZ;

        public override void Update()
        {
            base.Update();
            direction = Vector3.Normalize(Vector3.TransformNormal(Vector3.UnitZ, Transform.Mat));
        }

        public override Color GetLight(Vector3 pos, Vector3 view, Vector3 normal, Color surface)
        {
            var d = Vector3.Dot(normal, direction);
            var p = base.GetLight(pos, view, normal, surface) * (d < 0 ? -d : 0) + (Ambient * surface.G);
            var s = base.GetSpecular(direction, view, normal, surface);
            // 방향
            return p + s;
        }
    }

    public class PointLight : Light
    {
        public float Radius { get; set; }

        private Vector3 center;
        private float r2;

        public override void Update()
        {
            base.Update();
            // 빛의 위치를 World로
            center = Vector4.Transform(new Vector4(Vector3.Zero, 1), Transform.Mat).ToVector3();
            r2 = Radius * Radius;
        }

        public override Color GetLight(Vector3 pos, Vector3 view, Vector3 normal, Color surface)
        {
            var dir = pos - center;
            var l = dir.LengthSquared();
            if (r2 < l) return Color.Black;
            var d = Vector3.Dot(normal, Vector3.Normalize(dir));
            var p = base.GetLight(pos, view, normal, surface) * (d < 0 ? -d : 0);
            var s = base.GetSpecular(dir, view, normal, surface);
            return (p * (1 / l)) + s;
        }
    }
}
