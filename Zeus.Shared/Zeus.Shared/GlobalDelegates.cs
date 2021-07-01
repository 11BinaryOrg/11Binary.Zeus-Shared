
using Mercury.Shared;
namespace Zeus.Shared
{
    public static class GlobalDelegates
    {
        public   delegate void DataAvailableEventHandler(object Sender, DataReturnArgs e);
        public  delegate void SimpleMessageEventHandler(object Sender, SimpleMessageReturnArgs e);
        
    }
}
