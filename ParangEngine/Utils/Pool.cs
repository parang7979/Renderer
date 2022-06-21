using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    public class Poolable
    {
        public bool IsRelease { get; private set; } = true;

        virtual public void Acquire()
        {
            IsRelease = false;
        }

        virtual public void Release()
        {
            IsRelease = true;
        }
    }

    public class Pool<T> where T : Poolable, new()
    {
        private List<T> pool;

        public Pool(int capacity)
        {
            pool = new List<T>(capacity);
            for (int i = 0; i < capacity; i++) pool.Add(new T());
        }

        public T Acquire()
        {
            var ret = pool.FirstOrDefault(x => x.IsRelease);
            if (ret == null)
            {
                // ret = new T();
                // pool.Add(ret);
                return null;
            }
            ret.Acquire();
            return ret;
        }

        public List<T> Acquire(int count)
        {
            var ret = new List<T>();
            for (int i = 0; i < count; i++)
                ret.Add(Acquire());
            return ret;
        }
    }
}
