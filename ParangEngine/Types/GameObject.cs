using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    abstract public class Component
    {
        public Transform Transform { get; private set; }

        public Component(Transform transform)
        {
            Transform = transform;
        }

        virtual public void Update()
        {
        }
    }

    public class GameObject
    {
        public Transform Transform { get; private set; }

        public GameObject()
        {
            Transform = new Transform();
        }

        virtual public void Update()
        {
            Transform.Update();
        }
    }
}
