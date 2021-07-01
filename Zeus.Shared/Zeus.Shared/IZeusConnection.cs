using System;
using System.Collections.Generic;

namespace Zeus.Shared
{
    public interface IZeusConnection
    {
        ConnectionTypes.ConnType ConnectionType { get; set; }
            string ConnectionName { get; set; }
            System.Threading.CancellationToken token { get; set; }
            string NameSpace {get;set;}


            
            ConnectionTypes.Connect Connect { get; set; }

            ConnectionTypes.Location Location { get; set; }

            IDictionary<string, string> ConnectionURLS {get; set;}
            string SubscriptionName { get; set; }



    }
       
}
