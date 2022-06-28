using System;
using System.Windows.Forms;

namespace Renderer
{
    public partial class Renderer : Form
    {
        private Game.Game game;
        public Renderer()
        {
            InitializeComponent();

            Load += Renderer_Load;
            HandleDestroyed += Renderer_HandleDestroyed;
            KeyDown += Renderer_KeyDown;
            KeyUp += Renderer_KeyUp;

            game = new Game.Game(CreateGraphics(), ClientSize);
            
        }

        private void Renderer_Load(object sender, EventArgs e)
        {
            game.LoadResource();
            game.CreateScene();
            game.Start();
        }

        private void Renderer_HandleDestroyed(object sender, EventArgs e)
        {
            game.Stop();
        }

        private void Renderer_KeyDown(object sender, KeyEventArgs e)
        {
            game.KeyDown(e.KeyCode.ToString());
        }

        private void Renderer_KeyUp(object sender, KeyEventArgs e)
        {
            game.KeyUp(e.KeyCode.ToString());
        }
    }
}
