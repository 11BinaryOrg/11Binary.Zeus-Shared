using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus.Shared
{
    class TimeHelper
    {
        public static uint GetExpiry(uint tokenLifetimeInSeconds, DateTime fromDate)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = fromDate.ToUniversalTime() - origin;
            return Convert.ToUInt32(diff.TotalSeconds) + tokenLifetimeInSeconds;
        }
    }
}
