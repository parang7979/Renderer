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
        public Bitmap image;

        private BitmapData locked;

        public Texture(string path)
        {
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
                unsafe
                {
                    var ptr = (byte*)locked.Scan0;
                    var u = (int)(x * locked.Width);
                    var v = (int)(y * locked.Height);
                    var bit = locked.Stride / locked.Width;
                    var index = (v * locked.Width + u) * bit;
                    return new Color(ptr[index + 2], ptr[index + 1], ptr[index]);
                }
            }
            return Color.White; // 나중에 마젠타?
        }
    }
}
