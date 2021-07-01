using Mercury.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;
using System.Threading;


namespace Zeus.Shared
{
    public static class GlobalHelper
    {
        

        public   static async  Task<HttpRequestMessage> TranslateReceivedMessageToRequestMessage(ReceivedMessage rm)
        {
          
                try
                {
                    HttpRequestMessage req = new HttpRequestMessage();
                    
                    // Now load the Mercury.shared.ReceivedMessage into the dictionary
                    req.Properties.Add("Action", rm.Action);
                    req.Properties.Add("Application", rm.Application);
                    req.Properties.Add("ConnectionId", rm.ConnectionId);
                    req.Properties.Add("CreatedDt", rm.CreatedDt);
                    req.Properties.Add("Direction", rm.Direction);
                    req.Properties.Add("Entity", rm.Entity);
                    req.Properties.Add("NameSpace", rm.NameSpace);
                    req.Properties.Add("Originator", rm.Originator);
                    req.Properties.Add("Tenant", rm.Tenant);
                    req.Properties.Add("Version", rm.Version);
                    req.Properties.Add("WorkflowID", rm.WorkflowID.ToString());
                    req.Properties.Add("WorkflowInstanceID", rm.WorkflowInstanceID.ToString());
                    req.Properties.Add("WorkflowName", rm.WorkflowName);
                   
                    //Now lets setup the brokerted properties
                    Dictionary<string, object> bmDict = new Dictionary<string,object>();
                    bmDict.Add("To", rm.To);
                    bmDict.Add("CorelationId", rm.CorelationId);
                    bmDict.Add("ReplyTo", rm.ReplyTo);
                    bmDict.Add("SessionId", rm.SessionId);
                    string brokerproperties = JsonConvert.SerializeObject(bmDict);
                    req.Headers.Add("BrokerProperties", brokerproperties);
                    //req.Content = new StringContent(rm.Message);
   

                return req;
               }
                catch(Exception e)
                {
                    //ToDo: Create and respond with an error message
                    
                    return null;
                }

        }

        public static async  Task<Int32>  IsNull(object prop)
        {
          
                if(prop == null)
                {
                    return 1;
                }
                else
                { return 0; }
  
        }

        internal async static Task<string> TryRececeivedMessage(ReceivedMessage rm)
        {
            return await Task.Run<String>( () =>
            {
                Int32 flags = 0;
                Dictionary<string, Task<Int32>> tasksToRun = new Dictionary<string, Task<int>>();
                Task<Int32> task1 = IsNull(rm.Action);
                tasksToRun.Add("Action", task1);

                
                Task<Int32> task1a = IsNull(rm.MessageId);
                tasksToRun.Add("MessageID", task1a);

                Task<Int32> task1b = IsNull(rm.Token);
                tasksToRun.Add("Token", task1b);


                Task<Int32> task2 = IsNull(rm.Application);
                tasksToRun.Add("Application", task2);
                Task<Int32> task3 = IsNull(rm.Caller);
                tasksToRun.Add("Caller", task3);
                Task<Int32> task4 = IsNull(rm.ConnectionId);
                tasksToRun.Add("ConnectionId", task4);
                Task<Int32> task5 = IsNull(rm.CorelationId);
                tasksToRun.Add("CorelationId", task5);
                Task<Int32> task6 = IsNull(rm.CreatedDt);
                tasksToRun.Add("CreatedDt", task6);
                Task<Int32> task7 = IsNull(rm.Direction);
                tasksToRun.Add("Direction", task7);
                Task<Int32> task8 = IsNull(rm.Entity);
                tasksToRun.Add("Entity", task8);
            //Task<Int32> task9 = IsNull(rm.Message);
                //tasksToRun.Add("Message", task9);
                Task<Int32> task10 = IsNull(rm.NameSpace);
                tasksToRun.Add("NameSpace", task10);
                Task<Int32> task11 = IsNull(rm.Originator);
                tasksToRun.Add("Originator", task11);
                Task<Int32> task12 = IsNull(rm.ReplyTo);
                tasksToRun.Add("ReplyTo", task12);
                Task<Int32> task13 = IsNull(rm.SessionId);
                tasksToRun.Add("SessionId", task13);
                Task<Int32> task14 = IsNull(rm.Tenant);
                tasksToRun.Add("Tenant", task14);
                Task<Int32> task15 = IsNull(rm.To);
                tasksToRun.Add("To", task15);
                Task<Int32> task16 = IsNull(rm.WorkflowID);
                tasksToRun.Add("WorkflowID", task16);
                Task<Int32> task17 = IsNull(rm.WorkflowInstanceID);
                tasksToRun.Add("WorkflowInstanceID", task17);
                Task<Int32> task18 = IsNull(rm.WorkflowName);
                tasksToRun.Add("WorkflowName", task18);
                Task<Int32> task19 = IsNull(rm.Version);
                tasksToRun.Add("Version", task19);
                Task.WaitAll(tasksToRun.Values.ToArray());
                int p = 0;
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<String, Task<Int32>> t in tasksToRun)
                {
                    p = t.Value.Result;
                    if (p > 0)
                    {
                        sb.Append(t.Key);
                        sb.Append(", ");
                        flags++;
                    }
                }


                if (flags > 0)
                {
                    sb.Append(" fields were  null");
                    return sb.ToString();
                }
                else
                {
                    return "Pass";
                }
            });

        }

        

        internal static byte[] ComputeHmacSha256(byte[] secretKey, byte[] data)
        {
            HMACSHA256 temp = new HMACSHA256(secretKey);
            return temp.ComputeHash(data);
        }

        internal static string GenerateSasToken(string uri, string keyName, string key)
        {
            var dateFrom = DateTime.Now;

            var expiry = TimeHelper.GetExpiry(1200, dateFrom); // Set token lifetime to 20 minutes.

            string stringToSign = Uri.EscapeDataString(uri) + "\n" + expiry;
            var stringtoSignBytes = Encoding.UTF8.GetBytes(stringToSign);
            var keyBytes = Encoding.UTF8.GetBytes(key);

            //byte[] signatureBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            //signature = Convert.ToBase64String(signatureBytes
            string signature = Convert.ToBase64String(ComputeHmacSha256(keyBytes, stringtoSignBytes));

            string token = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                Uri.EscapeDataString(uri), Uri.EscapeDataString(signature), expiry, keyName);
            return token;
        }

        public static  IApplicationSettings GetConnectionInfo(string sharedAccessName, string sharedAccessKey,  string _tenant, string _application, string _sbname, string subscriptionName, string _version, string connType, bool localNameSpace, ConnectionTypes.Connect connect)
        {
            IZeusConnection conn = new ZeusConnection();
            IApplicationSettings settings = new ApplicationSettings();
            settings.Tenant = _tenant;
            settings.Application = _application;
            settings.BaseURI = String.Format("https://{0}.servicebus.windows.net", settings.Tenant);
            settings.ServiceBus_SharedKeyName = sharedAccessName;
            settings.ServiceBus_SharedKey = sharedAccessKey;
            settings.Port = "";
            settings.Version = _version;


            conn.ConnectionName = _sbname;
            conn.ConnectionType = GetConnectionType(connType);
            conn.SubscriptionName = subscriptionName;
            conn.NameSpace = String.Format("{0}.{1}", settings.Tenant, settings.Application);
           
            string tempURL = String.Format("https://{0}.servicebus.windows.net/{1}{2}/messages/head?timeout=60&api-version=z{3}", settings.Tenant, settings.Application, connType, settings.Version); ;
            
            conn.token = new CancellationToken(false);
            if (localNameSpace)
            {
                conn.Location = ConnectionTypes.Location.Local;
            }
            else
            {
                conn.Location = ConnectionTypes.Location.Remote;
            }

            conn.Connect = connect;
            settings.ZeusConnections.Add(conn);

            return settings;
        }

        private static ConnectionTypes.ConnType GetConnectionType(string connType)
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
            else if (connType.Contains("eventhub"))
            {
                conn = ConnectionTypes.ConnType.SBEventBus;
            }
            else 
            {
                conn = ConnectionTypes.ConnType.SBQueue;
            }

            return conn;
        }

        public  static ReceivedMessage CreateReceivedMessage()
        {

            ReceivedMessage rm = new ReceivedMessage();
            rm.Action = "";
            rm.Application = "";
            rm.Caller = "";
            rm.ConnectionId = Guid.NewGuid().ToString();
            rm.CorelationId = Guid.NewGuid().ToString();
            rm.CreatedDt = DateTime.Now.ToString();
            rm.Direction = "";
            rm.Entity = "";
            ActionRequest ar = new ActionRequest();
            ar.MachineName = "";
            ar.Message = "{Message: 'test message'}";
            ar.PageNumber = "1";
            ar.PageSize = "10";
            ar.Projection = "*";
            ar.Search = String.Empty; ;
            ar.SearchProperty = String.Empty;
            ar.SortColumn = string.Empty;
            ar.SortDirection = "asc";
            rm.Message = ar;
            rm.NameSpace = String.Empty;
            rm.Originator = String.Empty; 
            rm.ReplyTo = String.Empty; ;
            rm.MessageId = Guid.NewGuid().ToString();
            rm.Token = String.Empty;
            rm.SessionId = Guid.NewGuid().ToString();
            rm.Tenant = "OneOneBinary";
            rm.To = String.Empty; ;
            rm.Version = "1.0.0";
            rm.WorkflowID = Guid.Empty;
            rm.WorkflowInstanceID = Guid.Empty;
            rm.WorkflowName = String.Empty;
            return rm;


        }
    
    }
}
