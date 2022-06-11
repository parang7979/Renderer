using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public struct Screen
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int HalfWidth { get; private set; }
        public int HalfHeight { get; private set; }

        public int MinX { get; private set; }
        public int MaxX { get; private set; }

        public int MinY { get; private set; }
        public int MaxY { get; private set; }
        
        public float AspectRatio { get; private set; }

        public Screen(int width, int height)
        {
            Width = width - 1;
            Height = height - 1;

            HalfWidth = Width / 2;
            HalfHeight = Height / 2;

            MinX = 0;
            MaxX = Width;

            MinY = 0;
            MaxY = Height;

            AspectRatio = Width / (float)Height;
        }

        public Point Clamp(Point p)
        {
            p.X = Math.Max(MinX, Math.Min(p.X, MaxX));
            p.Y = Math.Max(MinY, Math.Min(p.Y, MaxY));
            return p;
        }

        private int Test(Point p)
        {
            int result = 0;
            if (p.X < 0) result |= 0b0001;
            else if (p.X > Width) result |= 0b0010;
            if (p.Y < 0) result |= 0b0100;
            else if (p.Y > Height) result |= 0b1000;
            return result;
        }

        public bool ClipLine(ref Point p1, ref Point p2)
        {
            int s1 = Test(p1);
            int s2 = Test(p2);

            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;
            for (int i = 0; i < 2; i++)
            {
                if (s1 == 0 && s2 == 0) return true;
                else if ((s1 & s2) > 0) return false;
                else
                {
                
                    Point p = new Point(0, 0);
                    int s = s1 != 0 ? s1 : s2;
                    if (s < 0b0100)
                    {
                        if ((s & 0b0001) > 0) p.X = MinX;
                        else p.X = MaxX;

                        if (dy == 0) p.Y = p1.Y;
                        else p.Y = p1.Y + dy * (p.X - p1.X) / dx;
                    }
                    else
                    {
                        if ((s & 0b0100) > 0) p.Y = MinY;
                        else p.Y = MaxY;

                        if (dx == 0) p.X = p1.X;
                        else p.X = p1.X + dx * (p.Y - p1.Y) / dy;
                    }
                    if (s1 != 0)
                    {
                        p1 = p;
                        s1 = Test(p1);
                    }
                    else
                    {
                        p2 = p;
                        s2 = Test(p2);
                    }
                }
            }
            return true;
        }

        public Point ToPoint(float x, float y, float w)
        {
            int px = (int)(HalfWidth - x);
            int py = (int)(HalfHeight - y);
            return new Point(px, py);
        }

        public Point ToPoint(Vector2 v)
        {
            return ToPoint(v.X, v.Y, 1);
        }

        public Point ToPoint(Vector4 v)
        {
            return ToPoint(v.X, v.Y, v.W);
        }

        public Point ToPoint(Vertex v)
        {
            return ToPoint(v.X, v.Y, v.W);
        }
    }
}
