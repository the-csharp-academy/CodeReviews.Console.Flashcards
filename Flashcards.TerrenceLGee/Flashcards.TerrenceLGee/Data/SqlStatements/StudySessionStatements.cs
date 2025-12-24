namespace Flashcards.TerrenceLGee.Data.SqlStatements;

public static class StudySessionStatements
{
    public static string InsertStudySession => $"""
                                                INSERT INTO {TableNames.StudySessionsTable}(StackId, TotalQuestions,
                                                Correct, Incorrect, Score, SessionDuration) 
                                                OUTPUT INSERTED.Id 
                                                VALUES(@StackId, @TotalQuestions, @Correct, @Incorrect, @Score, @SessionDuration);
                                                """;

    public static string GetStudySession => $"""
                                             SELECT * FROM 
                                             {TableNames.StudySessionsTable} LEFT JOIN {TableNames.SessionFlashcardsTable} ON 
                                             {TableNames.StudySessionsTable}.Id = {TableNames.SessionFlashcardsTable}.SessionId 
                                             WHERE {TableNames.StudySessionsTable}.Id = @Id AND StackId = @StackId ORDER BY 
                                             {TableNames.SessionFlashcardsTable}.DisplayPosition;
                                             """;

    public static string GetStudySessions => $"""
                                              SELECT * FROM
                                               {TableNames.StudySessionsTable} LEFT JOIN {TableNames.SessionFlashcardsTable} 
                                               ON {TableNames.StudySessionsTable}.Id = {TableNames.SessionFlashcardsTable}.SessionId 
                                               WHERE StackId = @StackId ORDER BY {TableNames.SessionFlashcardsTable}.DisplayPosition;
                                              """;

    public static string GetStudySessionCount => $"""
                                                  SELECT COUNT(*) FROM {TableNames.StudySessionsTable} WHERE StackId = @StackId;
                                                  """;
}