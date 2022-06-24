using ParangEngine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine
{
    static public class SceneManager
    {
        static public Scene CurrentScene => currentScene;

        private static Scene currentScene = null;

        static public void LoadScene(Scene scene)
        {
            currentScene = scene;
        }
    }
}
