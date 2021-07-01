using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus.Shared
{
    public static class ConnectionTypes
    {
        public enum ConnType
        {
            SBTopic = 0,
            SBQueue = 1,
            SBEventBus = 2
        };

        public enum Connect
        {
            Send = 0,
            Listen = 1,
            Manage = 2
        };

        public enum Location
        {
            Local = 0,
            Remote = 1
        }
    }
}
