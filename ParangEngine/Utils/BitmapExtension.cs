using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    static public class BitmapExtension
    {
        static public void SetPixel(this BitmapData b, int index, Color color)
        {
            if (index < 0 || b.Width * b.Height <= index) return;
            unsafe
            {
                var ptr = (byte*)b.Scan0;
                index *= 3;
                ptr[index] = color.BB;
                ptr[index + 1] = color.BG;
                ptr[index + 2] = color.BR;
            }
        }

        static public void SetPixel(this BitmapData b, int x, int y, Color color)
        {
            SetPixel(b, y * b.Width + x, color);
        }

        static public void Clear(this BitmapData b, Color color)
        {
            for (int i = 0; i < b.Height * b.Width; i++) SetPixel(b, i, color);
        }

        static public void Blend(this BitmapData dst, BitmapData src1, BitmapData src2)
        {
            unsafe
            {
                var d = (byte*)dst.Scan0;
                var p1 = (byte*)src1.Scan0;
                var p2 = (byte*)src2.Scan0;

                for (int i = 0; i < dst.Height * dst.Width * 3; i++)
                {
                    d[i] = (byte)((p1[i] / 255f) * (p2[i] / 255f) * 255f);
                }
            }
        }
    }
}
