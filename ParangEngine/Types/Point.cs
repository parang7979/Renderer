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

        public Vector2 ToVector2(Screen screen)
        {
            return new Vector2(screen.HalfWidth - X, screen.HalfHeight - Y);
        }   
    }
}
