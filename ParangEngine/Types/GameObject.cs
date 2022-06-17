using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    abstract public class Component
    {
        public Transform Transform { get; internal set; }

        virtual public void Update()
        {
            // TODO : Check Transform null
        }
    }

    public class GameObject
    {
        public Transform Transform { get; private set; }
        public List<Component> Components { get; private set; } = new List<Component>();

        public GameObject()
        {
            Transform = new Transform();
        }

        virtual public void Update()
        {
            Transform.Update();
            foreach (var c in Components) c.Update();
        }

        public void AddComponent(Component c)
        {
            c.Transform = Transform;
            Components.Add(c);
        }

        public T GetComponent<T>() where T : Component
        {
            return Components.FirstOrDefault(x => x is T) as T;
        }

        public bool RemoveComponent(Component c)
        {
            return Components.Remove(c);
        }
    }
}
