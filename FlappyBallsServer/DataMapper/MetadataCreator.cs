using DataMapper.model;

namespace DataMapper;

public class MetadataCreator
{
    public static Metadata GetPipesMetadata(string from,decimal[] pipes)
    {
        return new Metadata(RequestType.Pipes, from, pipes);
    }
}