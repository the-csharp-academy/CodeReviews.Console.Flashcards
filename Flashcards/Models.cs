namespace Flashcards;

public sealed record Stack(int Id, string Name);
public sealed record Flashcard(int Id, int StackId, string Front, string Back);

// The database StackId is intentionally omitted from the object shown to users.
// DisplayId is calculated with ROW_NUMBER so it always starts at 1 and has no gaps.
public sealed record FlashcardDto(int DisplayId, string Front, string Back);

public sealed record StudySessionDto(int Id, string StackName, DateTime StudiedAt, int Score, int TotalCards);

public sealed record MonthlyReportRow(string StackName, IReadOnlyList<decimal> Months);
