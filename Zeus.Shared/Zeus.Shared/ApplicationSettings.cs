using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus.Shared
{
    public class ApplicationSettings : IApplicationSettings
    {
        private string _tenant;
        private string _application;
        private string _baseURI;
        private string _servicebus_SharedKeyName;
        private string _serviceBus_SharedKey;
        private IDictionary<string, object> _applicationAuth;
        private string _port;
        private string _version;
        

        private List<IZeusConnection> _zeusConnections = new List<IZeusConnection>();


        public string Tenant
        {
            get
            {
                return _tenant;
            }
            set
            {
                _tenant = value;
            }
        }

        public string Application
        {
            get
            {
                return _application;
            }
            set
            {
                _application = value;
            }
        }

        public string BaseURI
        {
            get
            {
                return _baseURI;
            }
            set
            {
                _baseURI = value;
            }
        }

        public string ServiceBus_SharedKeyName
        {
            get
            {
                return _servicebus_SharedKeyName;
            }
            set
            {
                _servicebus_SharedKeyName = value;
            }
        }

        public string ServiceBus_SharedKey
        {
            get
            {
                return _serviceBus_SharedKey;
            }
            set
            {
                _serviceBus_SharedKey = value;
            }
        }

        public string Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }

        public IDictionary<string, object> ApplicationAuth
        {
            get
            {
                return _applicationAuth;
            }
            set
            {
                _applicationAuth = value;
            }
        }


        public List<IZeusConnection> ZeusConnections
        {
            get
            {
                return _zeusConnections;
            }
            set
            {
                _zeusConnections = value;
            }
        }

        

       

        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        

        
    }
}
