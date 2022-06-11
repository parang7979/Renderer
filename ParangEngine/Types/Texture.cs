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
    public class Texture
    {
        public int Id { get; private set; }

        private Bitmap image;
        private BitmapData locked;

        public Texture(int id, string path)
        {
            Id = id;
            image = new Bitmap(path, false);
            locked = null;
        }

        public void Lock()
        {
            if (locked == null)
            {
                locked = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            }
        }

        public void Unlock()
        {
            if (locked != null)
            {
                image.UnlockBits(locked);
                locked = null;
            }
        }

        public Color GetSample(Vector2 p)
        {
            return GetSample(p.X, p.Y);
        }

        public Color GetSample(float x, float y)
        {
            if (locked != null)
            {
                var u = (int)(x * locked.Width);
                var v = (int)(y * locked.Height);
                return locked.GetPixel(u, v);
            }
            return Color.White; // 나중에 마젠타?
        }
    }
}
