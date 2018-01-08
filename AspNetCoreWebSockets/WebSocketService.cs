using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreWebSockets
{
    public class WebSocketService : IWebSocketService
    {
        private Subject<(string, byte[])> _channel;

        public WebSocketService()
        {
            _channel = new Subject<(string, byte[])>();
        }

        public IObservable<(string, byte[])> Channel { get { return _channel.AsObservable(); } }

        public Task Send(string username, Message message)
        {
            _channel.OnNext(
                (
                    username,
                    Encoding.ASCII.GetBytes(
                        JsonConvert.SerializeObject(
                            message,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            })
                        )
                )
            );
            return Task.CompletedTask;
        }
    }
}
