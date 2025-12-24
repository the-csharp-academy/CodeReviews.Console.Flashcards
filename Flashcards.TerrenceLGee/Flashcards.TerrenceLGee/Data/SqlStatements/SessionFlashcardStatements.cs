namespace Flashcards.TerrenceLGee.Data.SqlStatements;

public static class SessionFlashcardStatements
{
    public static string InsertSessionFlashcard => $"""
                                                   INSERT INTO {TableNames.SessionFlashcardsTable}(SessionId, FlashcardId, Question, Answer, UserAnswer, IsCorrect, DisplayPosition) 
                                                   VALUES(@SessionId, @FlashcardId, @Question, @Answer, @UserAnswer, @IsCorrect, @DisplayPosition);
                                                   """;

    public static string GetSessionFlashcard => $"""
                                                 SELECT SessionId, FlashcardId, Question, Answer, UserAnswer, IsCorrect, DisplayPosition FROM 
                                                 {TableNames.SessionFlashcardsTable} WHERE SessionId = @SessionId AND FlashcardId = @FlashcardId;
                                                 """;

    public static string GetSessionFlashcards => $"""
                                                  SELECT SessionId, FlashcardId, Question, Answer, UserAnswer, IsCorrect, DisplayPosition FROM 
                                                  {TableNames.SessionFlashcardsTable} WHERE SessionId = @SessionId;
                                                  """;
}