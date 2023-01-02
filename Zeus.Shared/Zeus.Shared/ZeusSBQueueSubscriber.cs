using Mercury.Shared;
using System.Text.Json;
using System; 
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace Zeus.Shared
{
    public class ZeusSBQueueSubscriber : ZeusSubscriberBase
    {
        private bool _cancel = false;
        public CancellationTokenSource cts = new CancellationTokenSource();
        public override event GlobalDelegates.DataAvailableEventHandler OnDataAvailable;
        public override event EventHandler<Exception> OnExceptionThrown;
        public override event GlobalDelegates.SimpleMessageEventHandler OnFailure;
        public override event GlobalDelegates.SimpleMessageEventHandler OnSuccess;
       
        
        public ZeusSBQueueSubscriber(IApplicationSettings settings,  IZeusConnection conn) 
           :base(settings, conn)
        {
            // todo we need to send in the connection string  nad get the 
            // connection information from the ZeusConnection object
            Connection = conn;
            if (Connection == null)
            {
                
                Exception ex =  new Exception("No Queue connection found in ApplicationSettings");
                OnExceptionThrown(this, ex);
            }
            else
            {
                base.Connection = Connection;
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

        public  override async Task<bool> Receive()
        {

            if(Connection.Connect == ConnectionTypes.Connect.Send)
            {
                return false;
            }

            while (!_cancel)
            {
                try
                {


     
                        string fullAddressformat = "https://{0}.servicebus.windows.net/{1}/messages/head";
                        string fullAddress = String.Format(fullAddressformat, appSettings.Tenant, Connection.ConnectionName);
                        //string fullAddress = connection.ReceiveURLFormat;

                        using (var client = new HttpClient())
                        {
                            client.Timeout = Timeout.InfiniteTimeSpan;
                            string sasToken = GlobalHelper.GenerateSasToken(this.appSettings.BaseURI, this.appSettings.ServiceBus_SharedKeyName, this.appSettings.ServiceBus_SharedKey);
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sasToken);


                            var response = await client.DeleteAsync(fullAddress);

                            if (response.IsSuccessStatusCode)
                            {
                                var responseHeaders = response.Headers.ToList();
                                var contentType = response.Content.Headers.ContentType;
                                var encoding = response.Content.Headers.ContentEncoding;
                                string message = await response.Content.ReadAsStringAsync();
                                ReceivedMessage rm = JsonSerializer.Deserialize<ReceivedMessage>(message);
                                if (rm != null && message.Length > 0)
                                {
                                    DataReturnArgs args = new DataReturnArgs();
                                    args.Message = message;
                                    args.ReceivedMessage = rm;
                                    OnDataAvailable(this, args);
                                    // PCLTimer timer = new PCLTimer();
                                }
                                else if (!String.IsNullOrEmpty(message))
                                {
                                    DataReturnArgs args = new DataReturnArgs();
                                    args.Message = message;

                                    OnDataAvailable(this, args);
                                    // PCLTimer timer = new PCLTimer();
                                }



                            }

                        }
                        
                   
                }
                catch (Exception ex)
                {
                    OnExceptionThrown(this, ex);
                    

                }
                PCLTimer timer = new PCLTimer();
                timer.Delay(2000);
            }
            return true;
               
            

            
        }

        public override async Task<bool> Publish(ReceivedMessage rm)
        {
           
            if(Connection.Connect == ConnectionTypes.Connect.Listen)
            {
                return false;
            }
                try
                {

                    HttpRequestMessage req =
                            await GlobalHelper.TranslateReceivedMessageToRequestMessage(rm);
                    HttpClient client = new HttpClient();
                    string sasToken = GlobalHelper.GenerateSasToken(this.appSettings.BaseURI, this.appSettings.ServiceBus_SharedKeyName, this.appSettings.ServiceBus_SharedKey);
                    //client.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "appliation/json");
                    req.Headers.Add("Authorization", sasToken);
                    string data = JsonSerializer.Serialize(rm);


                    req.Content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                    req.Method = HttpMethod.Post;
                    //req.RequestUri = new Uri(connection.PublishURLFormat);

                    string fullAddressFormat = "https://{0}.servicebus.windows.net/{1}/messages?timeout=60&api-version={2}";
                    string fullAddress = string.Format(fullAddressFormat,base.appSettings.Tenant,  Connection.ConnectionName, base.appSettings.Version);
                    //string fullAddress = connection.PublishURLFormat;
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
               
                
            
            return true;

        }
        
    }
}
