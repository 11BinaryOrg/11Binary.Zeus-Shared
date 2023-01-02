using Mercury.Shared;
using System.Text.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;


namespace Zeus.Shared
{
    internal class ZeusSBEventHubSubscriber :ZeusSubscriberBase
    {
        private bool _cancel = false;
        public CancellationTokenSource cts = new CancellationTokenSource();
        public override event GlobalDelegates.DataAvailableEventHandler OnDataAvailable;
        public override event EventHandler<Exception> OnExceptionThrown;
        public override event GlobalDelegates.SimpleMessageEventHandler OnFailure;
        public override event GlobalDelegates.SimpleMessageEventHandler OnSuccess;

        public ZeusSBEventHubSubscriber(IApplicationSettings settings, IZeusConnection conn)
            : base(settings, conn)
        {
            // todo we need to send in the connection string  nad get the 
            // connection information from the ZeusConnection object
            Connection = conn;
            if (Connection == null)
            {

                Exception ex = new Exception("No EventHub  connection found in ApplicationSettings");
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

        public override async Task<bool> Receive()
        {
            NotImplementedException exc = new NotImplementedException("Event Hub Receivers are Limited to Servers");
            OnExceptionThrown(this, exc);
            return false;
        }

        public override async Task<bool> Subscribe()
        {
            return true;
        }

        public override async Task<bool> Publish(Mercury.Shared.ReceivedMessage rm)
        {
            if (Connection.Connect == ConnectionTypes.Connect.Listen)
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
                string fullAddress = string.Format(fullAddressFormat,appSettings.Tenant,  Connection.ConnectionName , appSettings.Version);
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
