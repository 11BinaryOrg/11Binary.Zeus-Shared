using Mercury.Shared;
using System.Text.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace Zeus.Shared
{
    public  class ZeusSBTopicSubscriber : ZeusSubscriberFactoryBase
    {
        private bool _cancel = false;
        public CancellationTokenSource cts = new CancellationTokenSource();
        public override event GlobalDelegates.DataAvailableEventHandler OnDataAvailable;
        public override event EventHandler<Exception> OnExceptionThrown;
        public override event GlobalDelegates.SimpleMessageEventHandler OnFailure;
        public override event GlobalDelegates.SimpleMessageEventHandler OnSuccess;
       
        public ZeusSBTopicSubscriber(IApplicationSettings settings, IZeusConnection conn) 
           :base(settings, conn)
        {
            IZeusConnection connection = conn;
            if (connection == null)
            {

                Exception ex = new Exception("No Topic connection found in ApplicationSettings");
                OnExceptionThrown(this, ex);
            }
            else
            {
                base.Connection = connection;
            }

        }        
        
       
        public override async Task<bool> Start(IZeusConnection az)
        {
           
            return true;
            
            
        }

        public override async Task<bool> Stop()
        {
            cts.Cancel();
                _cancel = true;
                return true;
            
           
 	 
        }






        public override async Task<bool> Receive()
        {
            while (!_cancel)
            {
                try
                {

                    //string fullAddress = "https://oneonebinary.servicebus.windows.net/medussatopic/subscriptions/medussa/messageapi-version=2014-01";
                    string formatAddress = "https://{0}.servicebus.windows.net/{1}/subscriptions/{2}/messages?api-version=2014-01";
                    string fullAddress = String.Format(formatAddress, appSettings.Tenant, Connection.ConnectionName, Connection.SubscriptionName);
                    using (var client = new HttpClient())
                    {
                        client.Timeout = Timeout.InfiniteTimeSpan;
                        string sasToken = GlobalHelper.GenerateSasToken(this.appSettings.BaseURI, this.appSettings.ServiceBus_SharedKeyName, this.appSettings.ServiceBus_SharedKey);
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sasToken);


                        var response = await client.GetAsync(fullAddress);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseHeaders = response.Headers.ToList();
                            var contentType = response.Content.Headers.ContentType;
                            var encoding = response.Content.Headers.ContentEncoding;
                            string message = await response.Content.ReadAsStringAsync();
                            if (!String.IsNullOrEmpty(message))
                            {


                                ReceivedMessage rm = JsonSerializer.Deserialize<ReceivedMessage>(message);
                                if (rm != null && message.Length >0)
                                {
                                    DataReturnArgs args = new DataReturnArgs();
                                    args.Message = message;
                                    args.CorelationID = rm.ConnectionId;
                                    args.SessionID = rm.SessionId;
                                    args.ReceivedMessage = rm;
                                    if(OnDataAvailable != null && args != null && this != null )
                                    {
                                        OnDataAvailable(this, args);
                                    }
                                    

                                }
                            }
                           



                        }

                    }
                    
                }
                catch (Exception ex)
                {
                    if(OnExceptionThrown != null)
                    {
                        OnExceptionThrown(this, ex);
                    }
                    
                    

                }
                
            }
            return true;


            
        }

        public override async Task<bool> Publish(ReceivedMessage rm)
        {
            try
            {

                HttpRequestMessage req =
                        await GlobalHelper.TranslateReceivedMessageToRequestMessage(rm);
               // HttpRequestMessage req = new HttpRequestMessage();
                HttpClient client = new HttpClient();
                string sasToken = GlobalHelper.GenerateSasToken(this.appSettings.BaseURI, this.appSettings.ServiceBus_SharedKeyName, this.appSettings.ServiceBus_SharedKey);
                //client.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "appliation/json");
                req.Headers.Add("Authorization", sasToken);
                string data = JsonSerializer.Serialize(rm);
                req.Content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                req.Method = HttpMethod.Post;
                //req.RequestUri = new Uri(connection.PublishURLFormat);

                string fullAddressformat = "https://{0}.servicebus.windows.net/{1}/messages";
                string fullAddress = String.Format(fullAddressformat,appSettings.Tenant,  Connection.ConnectionName);

                req.RequestUri = new Uri(fullAddress);
                HttpResponseMessage response = await client.SendAsync(req);
                response.EnsureSuccessStatusCode();
                string responseMessage = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(responseMessage))
                {
                    SimpleMessageReturnArgs args = new SimpleMessageReturnArgs();
                    args.OperationName = "Publish";
                    args.OperationResponse = String.Empty;
                    args.OperationMessage = data;
                    OnSuccess(this, args);
                    
                }
                else
                {
                    SimpleMessageReturnArgs args = new SimpleMessageReturnArgs();
                    args.OperationName = "Publish";
                    args.OperationResponse = responseMessage;
                    args.OperationMessage = data;
                    OnFailure(this, args);
                    
                }
                return true;
            }
            catch (Exception ex)
            {
                OnExceptionThrown(this, ex);
                return false;
                
            }
            
            
        }
    }
}
