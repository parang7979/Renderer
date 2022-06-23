using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Types
{
    abstract public class Component
    {
        public GameObject GameObject { get; internal set; }
        public Transform Transform => GameObject.Transform;

        virtual public void Update(int delta, List<string> keys)
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

        virtual public void Update(int delta, List<string> keys)
        {
            Transform.Update();
            foreach (var c in Components) c.Update(delta, keys);
        }

        public void AddComponent(Component c)
        {
            c.GameObject = this;
            Components.Add(c);
        }

        public T GetComponent<T>() where T : Component
        {
            return Components.FirstOrDefault(x => x is T) as T;
        }

        public List<T> GetComponents<T>() where T : Component
        {
            return Components.Where(x => x is T).Select(x => x as T).ToList();
        }

        public bool RemoveComponent(Component c)
        {
            return Components.Remove(c);
        }
    }
}
