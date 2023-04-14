public class Metadata
{
    public Metadata(RequestType requestType, string from, object value)
    {
        RequestType = requestType;
        From = from;
        Value = value;
    }

    public RequestType RequestType { get; }
    
    public string From { get; }

    public object Value { get; }
}