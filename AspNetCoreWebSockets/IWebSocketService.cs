using System;
using System.Threading.Tasks;

namespace AspNetCoreWebSockets
{
    public interface IWebSocketService
    {
        IObservable<(string, byte[])> Channel { get; }

        Task Send(string username, Message message);
    }
}