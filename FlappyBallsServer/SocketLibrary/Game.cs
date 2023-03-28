using System.Net.WebSockets;
using System.Text;
using DataMapper;
using DataMapper.model;
using static SocketLibrary.SocketAction;

namespace SocketLibrary;

public class Game
{
    private readonly Dictionary<WebSocket, String> _playerConnections;
    private readonly RequestHandler _requestHandler;
    private const int PipesAmount = 100;
    private readonly decimal[] _pipes;

    public Game()
    {
        _playerConnections = new Dictionary<WebSocket, string>();
        _pipes = new decimal[PipesAmount];
        Random random = new Random();
        for(int i = 0; i<PipesAmount; i++)
        {
            _pipes[i] = Math.Round((decimal)random.NextSingle(), 2);
        }
        _requestHandler = new RequestHandler(this);
        Task.Run(async () =>
        {
            while (true)
            {
                var toRemove = _playerConnections
                    .Keys
                    .Where((connection) => 
                        connection.State == WebSocketState.Aborted || connection.State == WebSocketState.Closed)
                    .ToArray();
                foreach (var key in toRemove) {
                    _playerConnections.Remove(key);
                }
                await Task.Delay(2000);
            }
        });
    }

    public void AddPlayer(WebSocket player)
    {
        _playerConnections.Add(player, "");
        Send(player, GetPipesMetadata(player));
        Task.Run(async () => {
            await Listen(player);
        });
    }

    public int GetPlayerCount()
    {
        return _playerConnections.Count;
    }

    private async Task Listen(WebSocket socket)
    {
        var buffer = new byte[1024];
        string response = string.Empty;
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
            response += Encoding.ASCII.GetString(buffer, 0, result.Count);
            if (result.EndOfMessage)
            {
                _requestHandler.HandleData(socket, response);
                response = string.Empty;
            }
        }
    }

    public async Task SendAll(Metadata data)
    {
        foreach (var player in _playerConnections)
        {
            if (player.Key.State == WebSocketState.Open)
            {
                await Send(player.Key, data);
            }
        }
    }

    public Metadata GetPipesMetadata(WebSocket socket)
    {
        return MetadataCreator.GetPipesMetadata("Server", _pipes);
    }

}