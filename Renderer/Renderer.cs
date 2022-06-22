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

            game = new Game.Game(CreateGraphics());
            
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
    }
}
