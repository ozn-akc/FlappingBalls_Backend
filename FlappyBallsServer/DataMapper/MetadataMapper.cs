using System.Text;
using System.Text.Json;
using DataMapper.model;
using Newtonsoft.Json;

namespace DataMapper;

public class MetadataMapper
{
    public static Metadata JsonToMetadata(String requestData)
    {
        return JsonConvert.DeserializeObject<Metadata>(requestData)!;
    }

    public static string MetadataToJson(Metadata metadata)
    {
        return JsonConvert.SerializeObject(metadata);
    }

    public static string DataToJson(object data)
    {
        return JsonConvert.SerializeObject(data);
    }

    public static byte[] EncodeJson(string json)
    {
        return Encoding.ASCII.GetBytes(json);
    }
}