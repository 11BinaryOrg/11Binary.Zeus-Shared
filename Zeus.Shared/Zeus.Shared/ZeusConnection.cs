using System;
using System.Collections.Generic;
using Mercury.Shared;
namespace Zeus.Shared
{
    public class ZeusConnection : IZeusConnection
    {
        private ConnectionTypes.ConnType _connType;
        private string _connectionname;
        private string _nameSpace;
        
        private System.Threading.CancellationToken _token;
        private ConnectionTypes.Connect _connect;
        private ConnectionTypes.Location _location;
        private string _subscriptionName;

        ConnectionTypes.ConnType IZeusConnection.ConnectionType
        {
            get
            {
                return _connType;
            }
            set
            {
                _connType = value;
            }
        }

        ConnectionTypes.Connect IZeusConnection.Connect
        {
            get
            {
                return _connect;
            }
            set
            {
                _connect = value;
            }
        }

        string IZeusConnection.ConnectionName
        {
            get
            {
                return _connectionname;
            }
            set
            {
                _connectionname = value;
            }
        }

        string IZeusConnection.NameSpace
        {
            get
            {
                return _nameSpace ;
            }
            set
            {
                _nameSpace = value;
            }
        }

        

        

        


        System.Threading.CancellationToken IZeusConnection.token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
            }
        }

        public ConnectionTypes.Location Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }

        public IDictionary<string, string> ConnectionURLS { get; set; }

        string IZeusConnection.SubscriptionName
        {
            get
            {
                return _subscriptionName;
            }
            set
            {
                _subscriptionName = value;
            }
        }


    }
}
