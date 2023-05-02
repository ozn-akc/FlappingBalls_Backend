using static MetadataMapper;

namespace SocketLibrary;

public class RequestHandler
{
    public static void HandleData(Game game, Player player, string data)
    {
        var metadata = JsonToMetadata(data);
        switch (metadata.RequestType)
        {
            case RequestType.Pipes:
                //Eine Pipe-Request bedeuted, dass ein Nutzer die Pipes erhalten möchte
                //obstaclteSpawner Spawn obstacle
                game.Send(player.Websocket, MetadataCreator.GetPipesMetadata(game.ServerName, game.GetPipes));
                break;
            case RequestType.JumpPlayer:
                break;
            case RequestType.JumpOther:
                break;
            case RequestType.DeathPlayer:
                game.SendAllButPlayer(player, MetadataCreator.GetDeathPlayerMetadata(game.ServerName, player.Name));
                break;
            case RequestType.DeathOther:
                break;
            case RequestType.Name:
                //Eine Name-Request bedeuted, dass ein Nutzer ihren Names setzen möchte 
                string name = (metadata.Value as string)!;
                //Wenn der Name schon vergeben ist, wird ein neuer Name angefordert
                if (game.PlayerNameExists(name))
                {
                    game.Send(player.Websocket, MetadataCreator.GetNameMetadata(game.ServerName));
                }
                //Ansonsten wird der name gesetzt evtl. auch ein NameAccepted als Antwort senden
                else
                {
                    player.Name = name;
                    game.Send(player.Websocket, MetadataCreator.GetNameSetMetadata(game.ServerName));
                }
                break;
            case RequestType.AllPlayerData:
                //Send Playerdata of the other players that are close to your player 
                game.Send(
                    player.Websocket, 
                    MetadataCreator.GetPlayerMetadata(
                        game.ServerName, 
                        game.GetPlayers
                            .Where(entry => entry!= player).ToList()
                        )
                    );
                break;
            case RequestType.Score:
                //Player Scored, so we increase their Score and inform all other Players
                player.IncreaseScore();
                game.SendAllButPlayer(player, new Metadata(RequestType.Highscore, player.Name, player.Score));
                break;
            case RequestType.Highscore:
                //Send the Player all Highscores. could also be send as only one Request
                foreach (var scores in game.GetPlayers)
                {
                    game.Send(player.Websocket, new Metadata(RequestType.Highscore, scores.Name, scores.Score));
                }
                break;
            case RequestType.Restart:
                player.Score = 0;
                player.Playtime = DateTime.Now;
                break;
        }
    }
}