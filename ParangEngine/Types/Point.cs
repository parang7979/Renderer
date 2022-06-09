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
    public struct ScreenSize
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        
        public int HalfWidth { get; private set; }
        public int HalfHeight { get; private set; }

        public int ScreenMinX { get; private set; }
        public int ScreenMaxX { get; private set; }

        public int ScreenMinY { get; private set; }
        public int ScreenMaxY { get; private set; }

        public ScreenSize(Image screen)
        {
            Width = screen.Width;
            Height = screen.Height;

            HalfWidth = screen.Width / 2;
            HalfHeight = screen.Height / 2;

            ScreenMinX = 0;
            ScreenMaxX = Width;

            ScreenMinY = 0;
            ScreenMaxY = Height;
        }
    }

    /// <summary>
    /// 화면의 중심이 0,0인 데카르트 좌표계로 변환해준다.
    /// </summary>
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(ScreenSize size, Vector2 v)
        {
            X = (int)(v.X + size.HalfWidth);
            Y = (int)(size.HalfHeight - v.Y);
        }

        public Point(ScreenSize size, Vertex v)
        {
            X = (int)(v.X + size.HalfWidth);
            Y = (int)(size.HalfHeight - v.Y);
        }

        public Vector2 ToVector2(ScreenSize size)
        {
            return new Vector2(X - size.HalfWidth, size.HalfHeight - Y);
        }

        private int Test(ScreenSize size)
        {
            int result = 0;
            if (X < 0) result |= 0b0001;
            else if (X > size.Width) result |= 0b0010;
            if (Y < 0) result |= 0b0100;
            else if (Y > size.Height) result |= 0b1000;
            return result;
        }

        static public bool ClipLine(ScreenSize size, ref Point p1, ref Point p2)
        {
            int s1 = p1.Test(size);
            int s2 = p2.Test(size);
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;

            if (s1 == 0 && s2 == 0) return true;
            else if ((s1 & s2) > 0) return false;
            else
            {
                Point p = new Point(0, 0);
                var s = s1 != 0 ? s1 : s2;
                if (s < 0b0100)
                {
                    if ((s & 0b0001) > 0) p.X = size.ScreenMinX;
                    else p.X = size.ScreenMaxX;

                    if (dy == 0) p.Y = p1.Y;
                    else p.Y = p1.Y + dy * (p.X - p1.X) / dx;
                }
                else
                {
                    if ((s & 0b0100) > 0) p.Y = size.ScreenMinY;
                    else p.Y = size.ScreenMaxY;

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
