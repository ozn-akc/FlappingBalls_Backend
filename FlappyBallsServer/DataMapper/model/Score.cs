namespace DataMapper.model;

public class Score
{
    public Score(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; set; }
    public int Value { get; set; }
}