using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    internal class Singleton<T> where T : class, new()
    {
        public T Instance
        {
            get
            {
                if (instance == null) instance = new T();
                return instance;
            }
        }

        private T instance = null;

        private Singleton()
        {

        }
    }
}
