using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus.Shared
{
    public class PCLTimer :IDisposable
    {
      public void Delay(int milliseconds)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
          while(watch.ElapsedMilliseconds <= milliseconds)
          {
              // Jut keep looping tuntil the timer is done
          }
          watch.Stop();
           
        }

      public void Dispose()
      {
          ((IDisposable)this).Dispose();
      }
    }
    
}
