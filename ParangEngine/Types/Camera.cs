using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class Camera
    {
        static private readonly Vector4 center = new Vector4(Vector3.Zero, 1f);
        static private readonly Vector4 xAxis = new Vector4(Vector3.UnitX, 1f);
        static private readonly Vector4 yAxis = new Vector4(Vector3.UnitY, 1f);
        static private readonly Vector4 zAxis = new Vector4(Vector3.UnitZ, 1f);

        public Transform Transform { get; private set; } = new Transform();
        public Screen Screen { get; private set; }
        public float Fov { get; set; } = 60f;
        public Color ClearColor { get; set; } = Color.Black;
        
        public Bitmap RenderTarget { get; set; }

        private BitmapData locked;
        private Matrix4x4 mat;

        public Camera(int width, int height, float fov)
        {
            Screen = new Screen(width, height);
            Fov = fov;
            RenderTarget = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        public void Lock()
        {
            if (locked == null)
            {
                // 뷰 메트릭스 생성
                mat = Matrix4x4.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);
                // Perspective
                mat *= Matrix4x4.CreatePerspectiveFieldOfView(Fov.ToRad(), Screen.AspectRatio, 1f, 100f);
                // 렌더 타겟 잠금
                locked = RenderTarget.LockBits(new Rectangle(0, 0, RenderTarget.Width, RenderTarget.Height), ImageLockMode.ReadWrite, RenderTarget.PixelFormat);
                // 컬러 클리어
                locked.Clear(ClearColor);
            }
        }

        public void Render(Vertex v1, Vertex v2, Vertex v3, Texture texture)
        {
            if (locked != null)
            {
                // 정점을 카메라 뷰에 맞게 변환
                v1 = ApplyView(v1);
                v2 = ApplyView(v2);
                v3 = ApplyView(v3);

                var edge1 = (v2.Pos - v1.Pos).ToVector3();
                var edge2 = (v3.Pos - v1.Pos).ToVector3();
                
                // cw backface culling
                var faceNormal = -Vector3.Cross(edge1, edge2);
                if (Vector3.Dot(faceNormal, Vector3.UnitZ) >= 0f)
                    return;

                // NDC to Screen
                v1 = ApplyScreen(v1);
                v2 = ApplyScreen(v2);
                v3 = ApplyScreen(v3);

                // 폴리곤
                locked.DrawPolygon(Screen, v1, v2, v3, texture);
                // 와이어프레임
                locked.DrawLine(Screen, v1, v2);
                locked.DrawLine(Screen, v2, v3);
                locked.DrawLine(Screen, v3, v1);
            }
        }

        public void RenderAxes(Transform transform)
        {
            if (locked != null)
            {
                var c = ApplyScreen(ApplyView(center * transform));
                var x = ApplyScreen(ApplyView(xAxis * transform));
                var y = ApplyScreen(ApplyView(yAxis * transform));
                var z = ApplyScreen(ApplyView(zAxis * transform));
                locked.DrawLine(Screen, c, x, new Color("red"));
                locked.DrawLine(Screen, c, y, new Color("green"));
                locked.DrawLine(Screen, c, z, new Color("blue"));
            }
        }

        public void Unlock()
        {
            if (locked != null)
            {
                RenderTarget.UnlockBits(locked);
                locked = null;
            }
        }

        public Vertex ApplyView(Vertex v)
        {
            v.Pos = ApplyView(v.Pos);
            return v;
        }

        public Vector4 ApplyView(Vector4 pos)
        {
            pos = Vector4.Transform(pos, mat);
            pos.Z = pos.Z == 0f ? float.MinValue : pos.Z;
            var invZ = 1f / pos.Z;
            pos.X *= invZ;
            pos.Y *= invZ;
            pos.Z *= invZ;
            return pos;
        }

        public Vertex ApplyScreen(Vertex v)
        {
            v.Pos = ApplyScreen(v.Pos);
            return v;
        }

        public Vector4 ApplyScreen(Vector4 pos)
        {
            pos.X *= Screen.HalfWidth;
            pos.Y *= Screen.HalfHeight;
            return pos;
        }
    }
}
