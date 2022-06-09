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

        public Point(Screen screen, Vector2 v)
        {
            X = (int)(v.X + screen.HalfWidth);
            Y = (int)(screen.HalfHeight - v.Y);
        }

        public Point(Screen screen, Vector4 v)
        {
            X = (int)(v.X + screen.HalfWidth);
            Y = (int)(screen.HalfHeight - v.Y);
        }

        public Point(Screen screen, Vertex v)
        {
            X = (int)(v.X + screen.HalfWidth);
            Y = (int)(screen.HalfHeight - v.Y);
        }

        public Vector2 ToVector2(Screen screen)
        {
            return new Vector2(X - screen.HalfWidth, screen.HalfHeight - Y);
        }   
    }
}
