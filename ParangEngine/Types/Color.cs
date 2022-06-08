using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public struct Color
    {
        public static readonly Color White = new Color(1f, 1f, 1f);
        public static readonly Color Black = new Color(0f, 0f, 0f);

        public float A { get; set; }
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }

        public byte BA => (byte)(A * 255f);
        public byte BR => (byte)(R * 255f);
        public byte BG => (byte)(G * 255f);
        public byte BB => (byte)(B * 255f);

        public Color(System.Drawing.Color color)
        {
            A = color.A / 255f;
            R = color.R / 255f;
            G = color.G / 255f;
            B = color.B / 255f;
            Clamp();
        }

        public Color(byte r, byte g, byte b)
        {
            A = 1f;
            R = r / 255f;
            G = g / 255f;
            B = b / 255f;
            Clamp();
        }

        public Color(float r, float g, float b)
        {
            A = 1f;
            R = r;
            G = g;
            B = b;
            Clamp();
        }

        public Color(string name)
        {
            var color = System.Drawing.Color.FromName(name);
            A = color.A / 255f;
            R = color.R / 255f;
            G = color.G / 255f;
            B = color.B / 255f;
            Clamp();
        }

        private void Clamp()
        {
            A = Math.Max(0f, Math.Min(A, 1f));
            R = Math.Max(0f, Math.Min(R, 1f));
            G = Math.Max(0f, Math.Min(G, 1f));
            B = Math.Max(0f, Math.Min(B, 1f));
        }

        static public Color operator *(Color c, float f)
        {
            Color r = c;
            r.A *= f;
            r.R *= f;
            r.G *= f;
            r.B *= f;
            r.Clamp();
            return r;
        }

        static public Color operator +(Color c1, Color c2)
        {
            Color r = c1;
            r.A += c2.A;
            r.R += c2.R;
            r.G += c2.G;
            r.B += c2.B;
            r.Clamp();
            return r;
        }
    }
}
