using System.Net.WebSockets;
using Newtonsoft.Json;

public class Player
{

    public Player(string name, double height, DateTime playtime, WebSocket websocket)
    {
        Name = name;
        Height = height;
        Playtime = playtime;
        Websocket = websocket;
    }
    public string Name { get; set; }
    public double Height { get; set; }
    public DateTime Playtime { get; set; }
    
    [JsonIgnore]
    public WebSocket Websocket { get; }
}