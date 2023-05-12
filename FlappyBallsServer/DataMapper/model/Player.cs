using System.Net.WebSockets;
using Newtonsoft.Json;

public class Player
{

    public Player(string name, double height, DateTime playtime, int score, WebSocket websocket)
    {
        Name = name;
        Height = height;
        Playtime = playtime;
        Websocket = websocket;
        Score = score;
        Dead = true;
    }
    public string Name { get; set; }
    public double Height { get; set; }
    public DateTime Playtime { get; set; }
    public int Score { get; set; }
    public int IncreaseScore() => Score += 1;
    
    
    [JsonIgnore]
    public WebSocket Websocket { get; }
    
    [JsonIgnore]
    public bool Dead { get; set; }
}