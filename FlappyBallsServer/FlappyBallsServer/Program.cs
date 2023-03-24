
using SocketLibrary;

SocketAction socketAction = new SocketAction();

var builder = new WebHostBuilder()
    .UseKestrel()
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
                        socketAction.Connect(context.WebSockets);
                        await Task.Run(() => {
                            while (true)
                            {
                            }
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
