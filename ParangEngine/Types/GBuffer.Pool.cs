using ParangEngine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    class DrawPixelsArg
    {
        public Screen Screen { get; set; }
        public OutputVS V1 { get; set; }
        public OutputVS V2 { get; set; }
        public OutputVS V3 { get; set; }
        public Material Material { get; set; }
        public Vector2 U { get; set; }
        public Vector2 V { get; set; }
        public Vector4 Dots { get; set; }
        public Vector3 InvZs { get; set; }
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }

        public void Setup(
            Screen screen, 
            OutputVS v1, OutputVS v2, OutputVS v3,
            Material material, Vector2 u, Vector2 v,
            Vector4 dots, Vector3 invZs,
            int minX, int maxX, int minY, int maxY)
        {
            Screen = screen;
            V1 = v1; V2 = v2; V3 = v3;
            Material = material;
            U = u; V = v;
            Dots = dots; InvZs = invZs;
            MinX = minX; MaxX = maxX; 
            MinY = minY; MaxY = maxY;
        }
    }

    class RenderPixelsArg
    {
        public BitmapData Bitmap { get; set; }
        public Screen Screen { get; set; }
        public Matrix4x4 InvPvMat { get; set; }
        public List<Light> Lights { get; set; }
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }

        public void Setup(
            BitmapData bitmap,
            Screen screen,
            Matrix4x4 invPvMat,
            List<Light> lights,
            int minX, int maxX, int minY, int maxY)
        {
            Bitmap = bitmap;
            Screen = screen;
            InvPvMat = invPvMat;
            Lights = lights;
            MinX = minX; MaxX = maxX;
            MinY = minY; MaxY = maxY;
        }
    }


    public partial class GBuffer
    {
        private List<DrawPixelsArg> drawPixelPool;
        private List<RenderPixelsArg> renderPixelPool;
    }
}
