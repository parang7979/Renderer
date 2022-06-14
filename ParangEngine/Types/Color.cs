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

        public bool IsBlack => (R + G + B) == 0;
        public float A { get; set; }
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }

        public byte BA => (byte)(A * byte.MaxValue);
        public byte BR => (byte)(R * byte.MaxValue);
        public byte BG => (byte)(G * byte.MaxValue);
        public byte BB => (byte)(B * byte.MaxValue);

        public ushort SA => (ushort)(A * ushort.MaxValue);
        public ushort SR => (ushort)(R * ushort.MaxValue);
        public ushort SG => (ushort)(G * ushort.MaxValue);
        public ushort SB => (ushort)(B * ushort.MaxValue);

        public Color(System.Drawing.Color color)
        {
            A = color.A / (float)byte.MaxValue;
            R = color.R / (float)byte.MaxValue;
            G = color.G / (float)byte.MaxValue;
            B = color.B / (float)byte.MaxValue;
            Clamp();
        }

        public Color(byte r, byte g, byte b)
        {
            A = 1f;
            R = r / (float)byte.MaxValue;
            G = g / (float)byte.MaxValue;
            B = b / (float)byte.MaxValue;
            Clamp();
        }

        public Color(byte a, byte r, byte g, byte b)
        {
            A = a / (float)byte.MaxValue;
            R = r / (float)byte.MaxValue;
            G = g / (float)byte.MaxValue;
            B = b / (float)byte.MaxValue;
            Clamp();
        }

        public Color(ushort r, ushort g, ushort b)
        {
            A = 1f;
            R = r / (float)ushort.MaxValue;
            G = g / (float)ushort.MaxValue;
            B = b / (float)ushort.MaxValue;
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

        public Color(float a, float r, float g, float b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
            Clamp();
        }

        public Color(string name)
        {
            var color = System.Drawing.Color.FromName(name);
            A = color.A / (float)byte.MaxValue;
            R = color.R / (float)byte.MaxValue;
            G = color.G / (float)byte.MaxValue;
            B = color.B / (float)byte.MaxValue;
            Clamp();
        }

        private void Clamp()
        {
            A = Math.Max(0f, Math.Min(A, 1f));
            R = Math.Max(0f, Math.Min(R, 1f));
            G = Math.Max(0f, Math.Min(G, 1f));
            B = Math.Max(0f, Math.Min(B, 1f));
        }

        static public Color operator *(Color c, float t)
        {
            return new Color(
                c.A * t,
                c.R * t,
                c.G * t,
                c.B * t);
        }

        static public Color operator *(Color c1, Color c2)
        {
            return new Color(
                c1.A * c2.A,
                c1.R * c2.R,
                c1.G * c2.G,
                c1.B * c2.B);
        }

        static public Color operator +(Color c1, float t)
        {
            return new Color(
                c1.A + t,
                c1.R + t,
                c1.G + t,
                c1.B + t);
        }

        static public Color operator +(Color c1, Color c2)
        {
            return new Color(
                c1.A + c2.A,
                c1.R + c2.R,
                c1.G + c2.G,
                c1.B + c2.B);
        }

        static public Color operator -(Color c1, float t)
        {
            return new Color(
                c1.A - t,
                c1.R - t,
                c1.G - t,
                c1.B - t);
        }

        static public Color operator -(Color c1, Color c2)
        {
            return new Color(
                c1.A - c2.A,
                c1.R - c2.R,
                c1.G - c2.G,
                c1.B - c2.B);
        }
    }
}
