using System.Net.WebSockets;

namespace SocketLibrary;

public class SocketAction
{
    private readonly List<Game> _games;

    public SocketAction()
    {
        _games = new List<Game>();
    }

    public async Task Connect(WebSocket webSocket)
    {
        Player player;
        //Set game amount to 20 max
        foreach (var game in _games.Where(game => game.GetPlayerCount() <= 10))
        {
            player = game.AddPlayer(webSocket);
            await game.Listen(player);
            return;
        }
        var newGame = new Game();
        player = newGame.AddPlayer(webSocket);
        _games.Add(newGame);
        await newGame.Listen(player);
    }
}