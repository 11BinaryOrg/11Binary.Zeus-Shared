using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
namespace Zeus.Shared
{
    public class ZeusSubscriberFactoryBase : IZeusSubscriber
    {
        public virtual  event GlobalDelegates.DataAvailableEventHandler OnDataAvailable;

        public virtual  event GlobalDelegates.SimpleMessageEventHandler OnSuccess;

        public virtual event GlobalDelegates.SimpleMessageEventHandler OnFailure;

        public virtual event EventHandler<Exception> OnExceptionThrown;
        public readonly IApplicationSettings appSettings;
        public IZeusConnection Connection { get; set; }

        protected HttpClient subscribeClient;
        protected HttpClient publishClient;

        public ZeusSubscriberFactoryBase(IApplicationSettings settings, IZeusConnection conn)
        {
            this.appSettings = settings;
            Connection = conn;
        }
        public virtual async  Task<bool> Receive()
        {
            return false;
        }

        public virtual async Task<bool> Start(IZeusConnection az)
        {
            return false;
            
        }

        public virtual async Task<bool> Stop()
        {
            return false;
        }

        public virtual async Task<bool> Subscribe()
        {
            return false;
        }

        public virtual async Task<bool> Unsubscribe()
        {
            return false;
        }

        public virtual async Task<bool> Publish(Mercury.Shared.ReceivedMessage rm)
        {
            return false ;
        }


       
    
    }
}
