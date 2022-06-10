using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ParangEngine.Utils;

namespace ParangEngine.Types
{
    public struct Frustum
    {
        public enum Result
        {
            Outside,
            Instersect,
            Inside,
        }

        private List<Plane> planes;

        public Frustum(Matrix4x4 mat)
        {
            mat = Matrix4x4.Transpose(mat);
            var matV = mat.SplitMatrix();
            planes = new List<Plane>()
            {
                new Plane(-(matV[3] - matV[1])),
                new Plane(-(matV[3] + matV[1])),
                new Plane(-(matV[3] - matV[0])),
                new Plane(-(matV[3] + matV[0])),
                new Plane(-(matV[3] - matV[2])),
                new Plane(-(matV[3] + matV[2])),
            };
        }

        public Result Check(Vector3 v)
        {
            foreach(var p in planes)
            {
                if (p.IsOutside(v)) return Result.Outside;
                else if (p.Distance(v) == 0f) return Result.Instersect;
            }
            return Result.Inside;
        }
    }
}
