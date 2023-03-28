using System.Net.WebSockets;
using System.Text;

namespace SocketLibrary;

public class Game
{
    private readonly List<WebSocket> _playerConnections;

    public Game()
    {
        _playerConnections = new List<WebSocket>();
        Task.Run(async () =>
        {
            while (true)
            {
                _playerConnections.RemoveAll((connection) => connection.State == WebSocketState.Aborted);
                _playerConnections.RemoveAll((connection) => connection.State == WebSocketState.Closed);
                await Task.Delay(2000);
            }
        });
    }

    public void AddPlayer(WebSocket player)
    {
        _playerConnections.Add(player);
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
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
            Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count));
            await SendAll("Answer from Server");
        }
    }

    public async Task SendAll(String message)
    {
        foreach (var player in _playerConnections)
        {
            if (player.State == WebSocketState.Open)
            {
                await Send(player, message);
            }
        }
    }

    private async Task Send(WebSocket socket, String message)
    {
        await socket.SendAsync(Encoding.ASCII.GetBytes(message), WebSocketMessageType.Text, true,
            CancellationToken.None);
    }

}