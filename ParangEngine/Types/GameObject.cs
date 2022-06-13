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
    }

    public class GameObject : Component
    {
        public GameObject() : base(new Transform())
        {

        }

        virtual public void Update()
        {
            Transform.Update();
        }
    }
}
