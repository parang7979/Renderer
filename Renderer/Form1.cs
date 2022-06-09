using ParangEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Renderer
{
    public partial class Form1 : Form
    {
        private Engine engine;

        public Form1()
        {
            InitializeComponent();

            Load += Form1_Load;
            HandleDestroyed += Form1_HandleDestroyed;
            engine = new Engine(CreateGraphics(), new Size(640, 480), 2);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            engine.Start();
        }

        private void Form1_HandleDestroyed(object sender, EventArgs e)
        {
            engine.Stop();
        }
    }
}
