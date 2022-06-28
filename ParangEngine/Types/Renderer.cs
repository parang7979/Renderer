using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    public class Renderer : Component
    {
        virtual public void Draw(List<Camera> cameras)
        {
        }
    }
}
