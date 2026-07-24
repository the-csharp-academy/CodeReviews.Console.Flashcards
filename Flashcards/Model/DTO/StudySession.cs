public sealed class StudySession
{
    public int SessionId { get; set; }
    public int StackId { get; set; }
    public int Score { get; init; }
    public int TotalQuestions { get; init; }
    public DateTime CompletedAt { get; init; }
}