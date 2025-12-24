namespace Flashcards.TerrenceLGee.Data.SqlStatements;

public static class StudyStackStatements
{
    public static string InsertStack => $"""

                                                 INSERT INTO {TableNames.StudyStacksTable}(Subject, Name) 
                                                 VALUES(@Subject, @Name);
                                         """;

    public static string UpdateStack => $"""

                                                 UPDATE {TableNames.StudyStacksTable} SET Subject = @Subject, 
                                                 Name = @Name WHERE Id = @Id;
                                         """;
    
    public static string DeleteStack => $"""
                                         DELETE FROM 
                                         {TableNames.StudyStacksTable} WHERE Id = @Id;
                                         """;

    public static string GetStack => $"""
                                      SELECT * FROM {TableNames.StudyStacksTable} LEFT JOIN 
                                              {TableNames.FlashcardsTable} ON {TableNames.StudyStacksTable}.Id = {TableNames.FlashcardsTable}.StackId 
                                              LEFT JOIN {TableNames.StudySessionsTable} ON {TableNames.StudyStacksTable}.Id = 
                                              {TableNames.StudySessionsTable}.StackId WHERE {TableNames.StudyStacksTable}.Id = @Id;
                                      """;

    public static string GetStacks => $"""
                                       SELECT * FROM {TableNames.StudyStacksTable} LEFT JOIN 
                                               {TableNames.FlashcardsTable} ON {TableNames.StudyStacksTable}.Id = {TableNames.FlashcardsTable}.StackId 
                                               LEFT JOIN {TableNames.StudySessionsTable} ON {TableNames.StudyStacksTable}.Id = 
                                               {TableNames.StudySessionsTable}.StackId;
                                       """;

    public static string GetStackCount => $"""
                                           SELECT COUNT(*) FROM {TableNames.StudyStacksTable};
                                           """;

    public static string GetStackNameAndId => $"""
                                               SELECT Id, Name FROM {TableNames.StudyStacksTable};
                                               """;

    public static string GetStackName = $"""
                                         SELECT Name FROM {TableNames.StudyStacksTable} WHERE Id = @Id;
                                         """;
}