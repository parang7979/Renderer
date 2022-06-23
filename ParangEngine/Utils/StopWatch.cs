using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParangEngine.Utils
{
    internal class StopWatch : IDisposable
    {
        public static Dictionary<string, long> Avrs = new Dictionary<string, long>();

        private string title;
        private System.Diagnostics.Stopwatch sw;

        public StopWatch(string title)
        {
#if DEBUG && STOP_WATCH
            this.title = title;
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif
        }

        public void Dispose()
        {
#if DEBUG && STOP_WATCH
            sw.Stop();
            if (sw.ElapsedMilliseconds > 1)
            {
                if (Avrs.ContainsKey(title)) Avrs[title] = (Avrs[title] + sw.ElapsedMilliseconds) / 2;
                else Avrs.Add(title, sw.ElapsedMilliseconds);
                Console.WriteLine($"{title} : {sw.ElapsedMilliseconds}ms");
            }
#endif
        }
    }
}
