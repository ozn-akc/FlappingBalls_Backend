namespace DataMapper.model;

public class Metadata
{
    public Metadata(RequestType requestType, string from, object value)
    {
        RequestType = requestType;
        From = from;
        Value = value;
    }

    public RequestType RequestType { get; set; }
    
    public string From { get; set; }

    public object Value { get; set; }
}