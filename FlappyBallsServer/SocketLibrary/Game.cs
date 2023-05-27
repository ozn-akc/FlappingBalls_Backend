using System.Net.WebSockets;
using System.Text;
using static SocketLibrary.RequestHandler;
using static MetadataCreator;
using static MetadataMapper;

namespace SocketLibrary;

public class Game
{
    private readonly List<Player> _playerConnections;
    private Pipes _pipes;
    public Pipes GetPipes => _pipes;
    public List<Player> GetPlayers => _playerConnections;
    public readonly string ServerName = "Server";
    private int counter = 0;
    
    public Game()
    {
        _playerConnections = new List<Player>();
        _pipes = new Pipes();
        Task.Run(async () =>
        {
            while (true)
            {
                _playerConnections.RemoveAll((connection) => 
                    connection.Websocket.State is not (WebSocketState.Open or WebSocketState.Connecting));
                await Task.Delay(1000);
            }
        });
    }

    public Player AddPlayer(WebSocket websocket)
    {
        Player player = new Player("player" + counter, 0,  DateTime.Now, 0, websocket);
        _playerConnections.Add(player);
        Send(player.Websocket, GetPipesMetadata(ServerName, _pipes));
        Send(
            player.Websocket, 
            GetPlayerMetadata(
                ServerName,
                GetPlayers.Where(entry => entry!= player && !entry.Dead).ToList()
                )
        );
        counter++;
        return player;
    }

    public int GetPlayerCount()
    {
        return _playerConnections.Count;
    }

    public async Task Listen(Player player)
    {
        var buffer = new byte[1024];
        string response = string.Empty;
        while (player.Websocket.State == WebSocketState.Open)
        {
            var result = await player.Websocket.ReceiveAsync(buffer, CancellationToken.None);
            response += Encoding.ASCII.GetString(buffer, 0, result.Count);
            if (result.EndOfMessage)
            {
                HandleData(this, player, response);
                response = string.Empty;
            }
        }
    }

    public async Task SendAll(Metadata data)
    {
        foreach (var player in _playerConnections)
        {
            if (player.Websocket.State == WebSocketState.Open)
            {
                await Send(player.Websocket, data);
            }
        }
    }

    public async Task SendAllButPlayer(Player original, Metadata data)
    {
        foreach (var player in _playerConnections)
        {
            if (player.Websocket.State == WebSocketState.Open && player.Name != original.Name)
            {
                await Send(player.Websocket, data);
            }
        }
    }

    public async Task Send(WebSocket socket, Metadata metadata)
    {
        await socket.SendAsync(EncodeJson(MetadataToJson(metadata)),
            WebSocketMessageType.Text, 
            true,
            CancellationToken.None);
    }

    public bool PlayerNameExists(string name)
    {
        return _playerConnections.Select(player => player.Name).Contains(name);
    }

}