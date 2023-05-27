using System.Net.WebSockets;
using System.Text;
using static SocketLibrary.RequestHandler;
using static MetadataCreator;
using static MetadataMapper;

namespace SocketLibrary;

//Game class contains Map and Players
public class Game
{
    //List of Players
    private readonly List<Player> _playerConnections;
    //Map with 100 pipes
    private Pipes _pipes;
    public Pipes GetPipes => _pipes;
    public List<Player> GetPlayers => _playerConnections;
    public readonly string ServerName = "Server";
    private int counter = 0;
    
    public Game()
    {
        //init Map and Playerlist
        _playerConnections = new List<Player>();
        _pipes = new Pipes();
        //Run async Task per game that deletes closed and aborted Connections
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

    //Add Player to Game
    public Player AddPlayer(WebSocket websocket)
    {
        //create Player with blank Data for Connection
        Player player = new Player("player" + counter, 0,  DateTime.Now, 0, websocket);
        _playerConnections.Add(player);
        //Send Player the Pipes(MapData)
        Send(player.Websocket, GetPipesMetadata(ServerName, _pipes));
        //Send Player all Alive Players
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

    //Listens to Player Websocket and calls Handlefunction
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
                //HandleData
                HandleData(this, player, response);
                response = string.Empty;
            }
        }
    }

    //Send data to All Players
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

    //Send to all Players except for the original Player
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

    //send Metadata to Socket
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