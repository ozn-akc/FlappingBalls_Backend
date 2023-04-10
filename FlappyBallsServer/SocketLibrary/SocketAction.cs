using System.Net.WebSockets;
using System.Text;
using DataMapper;
using DataMapper.model;
using Microsoft.AspNetCore.Http;

using static DataMapper.MetadataMapper;

namespace SocketLibrary;

public class SocketAction
{
    private readonly List<Game> _games;

    public SocketAction()
    {
        _games = new List<Game>();
    }

    public async Task Connect(WebSocketManager webSocket)
    {
        var player = await webSocket.AcceptWebSocketAsync();

        var added = false;
        foreach (var game in _games.Where(game => game.GetPlayerCount() < 100 && !added))
        {
            game.AddPlayer(player);
            added = true;
        }

        if (!added)
        {
            var game = new Game();
            game.AddPlayer(player);
            _games.Add(game);
        }
    }

    public static async Task Send(WebSocket socket, Metadata metadata)
    {
        await socket.SendAsync(EncodeJson(DataToJson(metadata)),
            WebSocketMessageType.Text, 
            true,
            CancellationToken.None);
    }
}