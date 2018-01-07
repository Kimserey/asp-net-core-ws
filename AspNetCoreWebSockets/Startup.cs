using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reactive;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;
using System.Reactive.Subjects;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Linq;

namespace AspNetCoreWebSockets
{
    public class Startup
    {
        // Default buffer size 4KB = 2^10 * 4
        const int DefaultBufferSize = 1024 * 4;

        static Subject<(byte[], WebSocketReceiveResult)> _channel = new Subject<(byte[], WebSocketReceiveResult)>();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod());

            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        await StartWebSocketConnection(context);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });

            app.UseFileServer();
            app.UseMvc();
        }

        private async Task StartWebSocketConnection(HttpContext context)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var subscription = _channel.Subscribe(async data =>
            {
                await webSocket.SendAsync(new ArraySegment<byte>(data.Item1, 0, data.Item2.Count), data.Item2.MessageType, data.Item2.EndOfMessage, CancellationToken.None);
            });

            var buffer = new byte[DefaultBufferSize];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                _channel.OnNext((buffer.Take(result.Count).ToArray(), result));
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            subscription.Dispose();
        }
    }
}
