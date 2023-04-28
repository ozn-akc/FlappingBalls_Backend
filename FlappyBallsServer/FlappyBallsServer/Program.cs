using System.Net.WebSockets;
using SocketLibrary;

SocketAction socketAction = new SocketAction();
var builder = new WebHostBuilder()
    .UseKestrel()
    .UseUrls("http://0.0.0.0:5000", "http://127.0.0.1:5293")
    .Configure(
        app =>
        {
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2)
            };

            app.UseWebSockets(webSocketOptions);

            app.Run(async (context) =>
            {
                if (context.Request.Path == "/")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        await Task.Run(async () =>
                        {
                            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            await socketAction.Connect(webSocket);
                        });
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }
            });
        })
    .Build();
builder.Run();
