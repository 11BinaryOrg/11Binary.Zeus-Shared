using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zeus.Shared
{
    public interface IApplicationSettings
    {
        //namespace
        string Tenant { get; set; }
        
        //queue name / 
        string Application { get; set; }
        string BaseURI{ get; set; }

        string ServiceBus_SharedKeyName { get; set; }

        string ServiceBus_SharedKey { get; set; }

        string Port { get; set; }

        string Version { get; set; }


        IDictionary<string, object> ApplicationAuth { get; set; }
        List<IZeusConnection> ZeusConnections {get;set;}
       
    }
}
