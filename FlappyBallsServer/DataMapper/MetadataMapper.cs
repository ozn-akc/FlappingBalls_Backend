using System.Text;
using Newtonsoft.Json;

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
    
    public static object JsonToData(String requestData)
    {
        return JsonConvert.DeserializeObject(requestData)!;
    }

    public static byte[] EncodeJson(string json)
    {
        return Encoding.ASCII.GetBytes(json);
    }
}