using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreWebSockets
{
    public class Startup
    {
        // Default buffer size 4KB = 2^10 * 4
        const int DefaultBufferSize = 1024 * 4;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<IWebSocketService, WebSocketService>();
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
                        await StartWebSocketConnection(
                            context, 
                            app.ApplicationServices.GetRequiredService<IWebSocketService>());
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

        private async Task StartWebSocketConnection(HttpContext context, IWebSocketService service)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

            var subscription = service.Channel
                .Where(c => c.Item1 == context.Request.Query["username"])
                .Select(c => c.Item2)
                .Subscribe(async data =>
            {
                await webSocket.SendAsync(new ArraySegment<byte>(data, 0, data.Length), 
                    WebSocketMessageType.Text, 
                    true, 
                    CancellationToken.None);
            });

            var buffer = new byte[DefaultBufferSize];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                // The only expected message coming from a WebSocket connection is a closure message
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            subscription.Dispose();
        }
    }
}
