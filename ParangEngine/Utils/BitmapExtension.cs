using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    static public class BitmapExtension
    {
        static public Color GetPixel(this BitmapData b, int index)
        {
            if (index < 0 || b.Width * b.Height <= index) return Color.White;
            unsafe
            {
                if (b.PixelFormat == PixelFormat.Format48bppRgb || b.PixelFormat == PixelFormat.Format64bppArgb)
                {
                    // short
                    var ptr = (ushort*)b.Scan0;
                    var k = (b.Stride / b.Width) / 2;
                    index *= k;
                    if (k == 3)
                        return new Color(ptr[index + 2], ptr[index + 1], ptr[index]);
                    else
                        return new Color(ptr[index + 3], ptr[index + 2], ptr[index + 1], ptr[index]);
                }
                else
                {
                    var ptr = (byte*)b.Scan0;
                    var k = b.Stride / b.Width;
                    index *= k;
                    if (k == 3)
                        return new Color(ptr[index + 2], ptr[index + 1], ptr[index]);
                    else
                        return new Color(ptr[index + 3], ptr[index + 2], ptr[index + 1], ptr[index]);
                }
            }
        }

        static public Color GetPixel(this BitmapData b, int x, int y)
        {
            return GetPixel(b, y * b.Width + x);
        }

        static public void SetPixel(this BitmapData b, int index, Color color)
        {
            if (index < 0 || b.Width * b.Height <= index) return;
            unsafe
            {
                if (b.PixelFormat == PixelFormat.Format48bppRgb || b.PixelFormat == PixelFormat.Format64bppArgb)
                {
                    var ptr = (ushort*)b.Scan0;
                    var k = (b.Stride / b.Width) / 2;
                    index *= k;
                    if (k == 3)
                    {
                        ptr[index] = color.SB;
                        ptr[index + 1] = color.SG;
                        ptr[index + 2] = color.SR;
                    }
                    else
                    {
                        ptr[index] = color.SB;
                        ptr[index + 1] = color.SG;
                        ptr[index + 2] = color.SR;
                        ptr[index + 3] = color.SA;
                    }
                }
                else
                {
                    var ptr = (byte*)b.Scan0;
                    var k = b.Stride / b.Width;
                    index *= k;
                    if (k == 3)
                    {
                        ptr[index] = color.BB;
                        ptr[index + 1] = color.BG;
                        ptr[index + 2] = color.BR;
                    }
                    else
                    {
                        ptr[index] = color.BB;
                        ptr[index + 1] = color.BG;
                        ptr[index + 2] = color.BR;
                        ptr[index + 3] = color.BA;
                    }
                }
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
    }
}
