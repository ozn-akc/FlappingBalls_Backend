using SocketLibrary;

public class MetadataCreator
{
    public static Metadata GetPipesMetadata(string from, Pipes pipes)
    {
        return new Metadata(RequestType.Pipes, from, pipes);
    }
    public static Metadata GetNameMetadata(string from)
    {
        return new Metadata(RequestType.Name, from,"" );
    }
    public static Metadata GetNameSetMetadata(string from)
    {
        return new Metadata(RequestType.NameSet, from, from );
    }
    public static Metadata GetPlayerMetadata(string from, List<Player> players)
    {
        return new Metadata(RequestType.AllPlayerData, from, players);
    }

    public static Metadata GetDeathPlayerMetadata(string from, string playerName)
    {
        return new Metadata(RequestType.DeathOther, from, playerName);
    }

    public static Metadata GetJumpPlayerMetadata(string from, double playerHeight)
    {
        return new Metadata(RequestType.JumpOther, from, playerHeight);
    }
}