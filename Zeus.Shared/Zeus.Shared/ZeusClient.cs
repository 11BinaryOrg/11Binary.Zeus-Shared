using Mercury.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
//using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using Microsoft.Practices.Unity;


namespace Zeus.Shared
{
    public  class ZeusClient : IZeusClient
    {        
        public  event GlobalDelegates.DataAvailableEventHandler OnDataAvailable;
        public event GlobalDelegates.SimpleMessageEventHandler OnSuccess;
        public event GlobalDelegates.SimpleMessageEventHandler OnFailure;
        public  event EventHandler<Exception> OnExceptionThrown;
        private  List<IZeusSubscriber> Subscribers = new List<IZeusSubscriber>();
        private string _sharedAccessName = "RootManageSharedAccessKey";
        private string _sharedAccessKey = "Lwz2LypPyail/v1+Zg6oXkzUnYaVBkX88gsyctYIf4A=";
        public List<string> LocalNameSpaces = new List<string>();

        public  IApplicationSettings appSettings;
        public List<IApplicationSettings> settingsList = new List<IApplicationSettings>();

        public ZeusClient()
        {
            
        }
        
        public async  Task<bool> Start(IApplicationSettings _settings, List<string> localNameSpaces)
        {
            LocalNameSpaces = localNameSpaces;
            try
            {
               
                     if (_settings != null)
                    {
                        appSettings = _settings;
                        settingsList.Add(_settings);
                    }
                    else
                    {
                        //ToDo: Throw and error and return
                        appSettings = this.GetConnectionInfo("OneOneBinary", "Medusa", "2014-01", "queue");
                        settingsList.Add(appSettings);
                     }
                     
                    return await StartConnections();
                    
                }
            catch(Exception ex)
            {
                OnExceptionThrown(this, ex);
                return false;
            }
            
            
        }

        private async Task<bool> StartConnections()
        {
            foreach(IApplicationSettings app in settingsList)
            {
                foreach(IZeusConnection conn in app.ZeusConnections)
                {
                    IZeusSubscriber medussaSubscriber;
                    if(conn.ConnectionType == ConnectionTypes.ConnType.SBQueue)
                    {
                        medussaSubscriber = new ZeusSBQueueSubscriber(app, conn);
                    }
                    else if(conn.ConnectionType == ConnectionTypes.ConnType.SBTopic)
                    {
                        medussaSubscriber = new ZeusSBTopicSubscriber(app, conn);
                    }
                    else if (conn.ConnectionType == ConnectionTypes.ConnType.SBEventBus)
                    {
                        medussaSubscriber = new ZeusSBEventHubSubscriber(app, conn);
                    }
                    else
                    {
                        throw new Exception("Connection type is invalid");
                    }
                    
                    medussaSubscriber.OnDataAvailable += medussaSubscriber_OnDataAvailable;
                    medussaSubscriber.OnSuccess += medussaSubscriber_OnSuccess;
                    medussaSubscriber.OnFailure += medussaSubscriber_OnFailure;
                    medussaSubscriber.OnExceptionThrown += medussaSubscriber_OnExceptionThrown;
                    IZeusConnection sub = appSettings.ZeusConnections.FirstOrDefault();


                   
                        sub.token = new CancellationToken(false);
                        Subscribers.Add(medussaSubscriber);
                        await   medussaSubscriber.Start(sub);

                      

                    
                     
                }
                
            }
            return true;
        }

        public async Task<bool> Subscribe()
        {
            foreach(IZeusSubscriber sub in Subscribers)
            {
                if(sub.Connection.Connect == ConnectionTypes.Connect.Manage || sub.Connection.Connect == ConnectionTypes.Connect.Listen)
                {
                    await sub.Receive();
                }
               
            }
            return true;
        }
        void medussaSubscriber_OnExceptionThrown(object sender, Exception e)
        {
            if(OnExceptionThrown != null)
            {
                OnExceptionThrown(sender, e);
            }

            
        }

        void medussaSubscriber_OnFailure(object Sender, SimpleMessageReturnArgs e)
        {
            if(OnFailure != null)
            {
                OnFailure(Sender, e);
            }
           
        }

        void medussaSubscriber_OnSuccess(object Sender, SimpleMessageReturnArgs e)
        {
            if(OnSuccess != null)
            {
                OnSuccess(Sender, e);
            }
          
        }

        void medussaSubscriber_OnDataAvailable(object Sender, DataReturnArgs e)
        {
            
           
            if(OnDataAvailable != null)
            {
                OnDataAvailable(Sender, e);
            }
            
            
            
        }


         public async Task<bool> PublishMessage(string subscription, ReceivedMessage rm, ConnectionTypes.ConnType type)
        {
            if (!LocalNameSpaces.Contains(rm.NameSpace))
            {


                try
                {
                    //ToDo clear this up to pick the right subscriber to puvblish the message on
                    foreach (IApplicationSettings app in settingsList)
                    {
                        foreach (IZeusSubscriber sub in Subscribers)
                        {

                            if (sub.Connection.Connect == ConnectionTypes.Connect.Manage
                                || sub.Connection.Connect == ConnectionTypes.Connect.Send)
                            {
                                await sub.Publish(rm);
                                SimpleMessageReturnArgs args = new SimpleMessageReturnArgs();
                                args.OperationName = "Publish";
                                args.OperationMessage = "success";
                                OnSuccess(this, args);
                            }
                            else if (sub.Connection.Connect == ConnectionTypes.Connect.Manage
                                || sub.Connection.Connect == ConnectionTypes.Connect.Send)

                            { }
                        }
                    }



                    return true; ;

                }
                catch (Exception ex)
                {
                    OnExceptionThrown(this, ex);
                    return false;
                }
            }
            else
            {
                await PublishMessgeLocally(rm);
                return true; 
            }
        }

        private    IApplicationSettings GetConnectionInfo(string _tenant, string _application, string _version, string connType)
        {
            IZeusConnection conn = new ZeusConnection();
            IApplicationSettings settings = new ApplicationSettings();
            settings.Tenant = _tenant;
            settings.Application = _application;
            settings.BaseURI = String.Format("https://{0}.servicebus.windows.net",settings.Tenant);
            settings.ServiceBus_SharedKeyName = _sharedAccessName;
            settings.ServiceBus_SharedKey = _sharedAccessKey;
            settings.Port = "";
            settings.Version = _version;
            

            conn.ConnectionName = String.Format("{0}_{1}_{2}", settings.Tenant, settings.Application, connType);
           conn.ConnectionType = GetConnectionType(connType);
           //https://oneonebinary.servicebus.windows.net/medussaqueue/messages?timeout=60&api-version=2014-01
            conn.NameSpace = String.Format("{0}.{1}", settings.Tenant, settings.Application);

            
           
            conn.token = new CancellationToken(false);
            if(LocalNameSpaces.Contains(conn.NameSpace) )
            {
                conn.Location = ConnectionTypes.Location.Local;
            }
            else
            {
                conn.Location = ConnectionTypes.Location.Remote;
            }
            
            conn.Connect = ConnectionTypes.Connect.Manage;
            settings.ZeusConnections.Add(conn);

            return settings;
        }

        private ConnectionTypes.ConnType GetConnectionType(string connType)
        {
            ConnectionTypes.ConnType conn = new ConnectionTypes.ConnType();
            if (connType.Contains("queue"))
            {
                conn = ConnectionTypes.ConnType.SBQueue;
            }
            else if (connType.Contains("topic"))
            {
                conn = ConnectionTypes.ConnType.SBTopic;
            }
            else
            {
                conn = ConnectionTypes.ConnType.SBEventBus;
            }

            return conn;
        }








        public async Task<bool> PublishMessageOnSecificConnection(string connectionname, ReceivedMessage rm, ConnectionTypes.ConnType type)
        {
            try
            {
                IZeusSubscriber sub = Subscribers.Where(p => p.Connection.ConnectionName == connectionname && p.Connection.ConnectionType == type).FirstOrDefault();
                if (sub != null && (sub.Connection.Connect == ConnectionTypes.Connect.Manage
                                || sub.Connection.Connect == ConnectionTypes.Connect.Send))
                {
                    await sub.Publish(rm);
                    SimpleMessageReturnArgs args = new SimpleMessageReturnArgs();
                    args.OperationName = "PublishSpecifcConnection";
                    args.OperationMessage = "success";
                    args.OperationResponse = "Message published on specific connection successfully";
                    OnSuccess(this, args);
                }
                return true;
            }
            catch(Exception ex)
            {
                OnExceptionThrown(this, ex);
                return false;
            }
        }

        public async Task<bool> PublishMessgeLocally(ReceivedMessage rm)
        {
            try
            {


                DataReturnArgs args = new DataReturnArgs();
                args.ReceivedMessage = rm;
                args.CorelationID = rm.CorelationId;
                args.Message = JsonConvert.SerializeObject(rm);
                args.SessionID = rm.SessionId;
                OnDataAvailable(this, args);
                SimpleMessageReturnArgs sargs = new SimpleMessageReturnArgs();
                sargs.OperationName = "PublishLocalConnection";
                sargs.OperationMessage = "success";
                sargs.OperationResponse = "Message published on local connection successfully";
                OnSuccess(this, sargs);
                return true;
            }
            catch(Exception ex)
            {
                OnExceptionThrown(this, ex);
                return false;
            }

        }
    }
}
