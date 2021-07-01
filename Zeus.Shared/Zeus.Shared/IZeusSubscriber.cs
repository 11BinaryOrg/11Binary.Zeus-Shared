using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercury.Shared;

namespace Zeus.Shared
{
    public interface IZeusSubscriber
    {
        event GlobalDelegates.DataAvailableEventHandler OnDataAvailable;
        event GlobalDelegates.SimpleMessageEventHandler OnSuccess;
        event GlobalDelegates.SimpleMessageEventHandler OnFailure;
        event EventHandler<Exception> OnExceptionThrown;
        IZeusConnection Connection { get; set; }

        Task<bool> Start(IZeusConnection az);
        Task<bool> Stop();
        Task<bool> Subscribe();
        Task<bool> Receive();
        Task<bool> Unsubscribe();
        Task<bool> Publish(ReceivedMessage rm);
       

    }
}
