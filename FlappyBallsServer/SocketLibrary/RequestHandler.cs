using System.Net.WebSockets;
using DataMapper;
using DataMapper.model;
using static DataMapper.MetadataMapper;
using static DataMapper.MetadataCreator;
using static SocketLibrary.SocketAction;

namespace SocketLibrary;

public class RequestHandler
{
    private Game _game;

    public RequestHandler(Game game)
    {
        this._game = game;
    }
    public async void HandleData(WebSocket client, string data)
    {
        Metadata metadata = JsonToMetadata(data);
        switch (metadata.RequestType)
        {
            case RequestType.Pipes:
                //Send 
                await Send(client, _game.GetPipesMetadata(client));
                break;
            case RequestType.JumpPlayer:
                //We Receive a Jump from a player 
                break;
            case RequestType.JumpOther:
                //We Receive a Jump from someone else(Opp)
                break;
            case RequestType.DeathPlayer:
                //We Receive a Players Death
                break;
            case RequestType.DeathOther:
                //We Receive an Opps Death
                break;
        }
    }
}