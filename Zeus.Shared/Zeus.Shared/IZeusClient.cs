using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercury.Shared;

namespace Zeus.Shared
{
    public interface IZeusClient
    {

        event GlobalDelegates.DataAvailableEventHandler OnDataAvailable;
        event GlobalDelegates.SimpleMessageEventHandler OnSuccess;
        event GlobalDelegates.SimpleMessageEventHandler OnFailure;
        event EventHandler<Exception> OnExceptionThrown;
        Task<bool> Start(IApplicationSettings _settings, List<string> localNameSpaces);
        Task<bool> Subscribe();
        
        Task<bool> PublishMessage(string channel, ReceivedMessage rm, ConnectionTypes.ConnType type);
        Task<bool> PublishMessageOnSecificConnection(string connectionname, ReceivedMessage rm, ConnectionTypes.ConnType type);
    }
}
