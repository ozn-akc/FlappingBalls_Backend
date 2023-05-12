using System.Net.WebSockets;
using DataMapper.model;
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
                player.Height = (double)metadata.Value;
                game.SendAllButPlayer(player, MetadataCreator.GetJumpPlayerMetadata(player.Name, player.Height));
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
                sendHighscores(game);
                break;
            case RequestType.Highscore:
                sendHighscore(game, player.Websocket);
                break;
            case RequestType.Restart:
                player.Score = 0;
                player.Playtime = DateTime.Now;
                game.Send(
                    player.Websocket,
                    MetadataCreator.GetPlayerMetadata(
                        game.ServerName, 
                        game.GetPlayers
                            .Where(entry => entry!= player).ToList()
                    )
                );
                game.SendAllButPlayer(
                    player,
                    MetadataCreator.GetPlayerMetadata(
                        game.ServerName,
                        new List<Player> { player }
                    ));
                break;
        }
    }

    private static void sendHighscores(Game game)
    {
        List<Score> scores = game.GetPlayers
            .ConvertAll(play => new Score(play.Name, play.Score));
        scores.Sort((a, b) => b.Value.CompareTo(a.Value));
        //Send the Player all Highscores. could also be send as only one Request
        game.SendAll(new Metadata(
            RequestType.Highscore,
            game.ServerName,
            scores));
    }

    private static void sendHighscore(Game game, WebSocket socket)
    {
        List<Score> scores = game.GetPlayers
            .ConvertAll(play => new Score(play.Name, play.Score));
        scores.Sort((a,b) => b.Value.CompareTo(a.Value));
        //Send the Player all Highscores. could also be send as only one Request
        game.Send(socket, new Metadata(
            RequestType.Highscore, 
            game.ServerName, 
            scores));
    }
}