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
    public struct Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Pixel(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Pixel(Vector2 v)
        {
            X = (int)v.X;
            Y = (int)v.Y;
        }

        public Pixel(Vertex v)
        {
            X = (int)v.X;
            Y = (int)v.Y;
        }

        public int TestPixel(BitmapData b)
        {
            int result = 0;
            if (X < 0) result |= 0b0001;
            else if (X > b.Width) result |= 0b0010;
            if (Y < 0) result |= 0b0100;
            else if (Y > b.Height) result |= 0b1000;
            return result;
        }

        static public bool ClipLine(BitmapData b, ref Pixel p1, ref Pixel p2)
        {
            int s1 = p1.TestPixel(b);
            int s2 = p2.TestPixel(b);
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;

            if (s1 == 0 && s2 == 0) return true;
            else if ((s1 & s2) > 0) return false;
            else
            {
                Pixel p = new Pixel(0, 0);
                var s = s1 != 0 ? s1 : s2;
                if (s < 0b0100)
                {
                    if ((s & 0b0001) > 0) p.X = 0;
                    else p.X = b.Width - 1;

                    if (dy == 0) p.Y = p1.Y;
                    else p.Y = p1.Y + dy * (p.X - p1.X) / dx;
                }
                else
                {
                    if ((s & 0b0100) > 0) p.Y = 0;
                    else p.Y = b.Height - 1;

                    if (dx == 0) p.X = p1.X;
                    else p.X = p1.X + dx * (p.Y - p1.Y) / dy;
                }
                if (s1 != 0) p1 = p;
                else p2 = p;
            }
            return true;
        }
    }
}
