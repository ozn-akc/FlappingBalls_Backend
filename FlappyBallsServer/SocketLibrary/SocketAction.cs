using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;

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
}