public class CardStack
{
    public long StackId;
    public string Name { get; set; } = string.Empty;
}

public class CardStackDTO
{
    public string Name { get; set; } = string.Empty;
    public int CardCount { get; set; } = 0;
}