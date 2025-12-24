namespace Flashcards.TerrenceLGee.Data.SqlStatements;

public static class TableNames
{
    public static string StudyStacksTable => @"flashcardsDB.dbo.stacks";
    public static string FlashcardsTable => @"flashcardsDB.dbo.flashcards";
    public static string StudySessionsTable => @"flashcardsDB.dbo.sessions";
    public static string SessionFlashcardsTable => @"flashcardsDB.dbo.session_flashcards";
}