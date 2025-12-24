namespace Flashcards.TerrenceLGee.Data.SqlStatements;

public static class FlashcardStatements
{
    public static string InsertFlashcard => $"""
                                             INSERT INTO {TableNames.FlashcardsTable}(StackId, Question, Answer, Position) 
                                             VALUES(@StackId, @Question, @Answer, @Position);
                                             """;

    public static string UpdateFlashcard => $"""
                                             UPDATE {TableNames.FlashcardsTable} SET Question = @Question, Answer = 
                                             @Answer, Position = @Position WHERE Id = @Id AND StackId = @StackId;
                                             """;

    public static string DeleteFlashcard => $"""
                                             DELETE FROM {TableNames.FlashcardsTable} WHERE Position = @Position AND StackId = 
                                             @StackId;
                                             """;

    public static string GetFlashcard => $"""
                                          SELECT * FROM {TableNames.FlashcardsTable} 
                                          WHERE Position = @Position AND StackId = @StackId;
                                          """;

    public static string GetFlashcards => $"""
                                           SELECT * FROM {TableNames.FlashcardsTable} 
                                           WHERE StackId = @StackId ORDER BY Position;
                                           """;

    public static string GetFlashcardCount => $"""
                                               SELECT COUNT(*) FROM {TableNames.FlashcardsTable} WHERE StackId = @StackId;
                                               """;
}