public sealed class StudySession
{
    public int SessionId { get; set; }
    public int StackId { get; set; }
    public int Score { get; init; }
    public int TotalQuestions { get; init; }
    public DateTime CompletedAt { get; init; }
}

public sealed class StudySessionDTO
{
    public string StackName { get; set; } = string.Empty;
    public int Score { get; init; }
    public int TotalQuestions { get; init; }
    public DateTime CompletedAt { get; init; }

    public double Percentage =>
        TotalQuestions == 0
            ? 0
            : (double)Score / TotalQuestions * 100;
}