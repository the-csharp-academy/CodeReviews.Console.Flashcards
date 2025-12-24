namespace Flashcards.TerrenceLGee.Data.SqlStatements;

public static class DatabaseInitializationStatements
{
    public static string CreateDatabaseStatement => """

                                                            IF NOT EXISTS(SELECT name FROM sys.databases WHERE name = 'flashcardsDB') 
                                                            CREATE DATABASE flashcardsDB;
                                                    """;

    public static string CreateStudyStackTable => $"""

                                                           IF OBJECT_ID(N'{TableNames.StudyStacksTable}', N'U') IS NULL 
                                                           CREATE TABLE {TableNames.StudyStacksTable} (
                                                               Id INT IDENTITY(1,1) PRIMARY KEY,
                                                               Subject INT NOT NULL,
                                                               Name NVARCHAR(300));
                                                   """;

    public static string CreateFlashcardTable => $"""

                                                          IF OBJECT_ID(N'{TableNames.FlashcardsTable}', N'U') IS NULL 
                                                          CREATE TABLE {TableNames.FlashcardsTable} (
                                                              Id INT IDENTITY(1,1) PRIMARY KEY,
                                                              StackId INT NOT NULL,
                                                              Question NVARCHAR(2000) NOT NULL,
                                                              Answer NVARCHAR(2000) NOT NULL,
                                                              Position INT NOT NULL,
                                                              FOREIGN KEY (StackId) REFERENCES {TableNames.StudyStacksTable} (Id) ON DELETE CASCADE);
                                                  """;

    public static string CreateStudySessionTable => $"""

                                                             IF OBJECT_ID(N'{TableNames.StudySessionsTable}', N'U') IS NULL 
                                                             CREATE TABLE {TableNames.StudySessionsTable} (
                                                                 Id INT IDENTITY(1,1) PRIMARY KEY,
                                                                 StackId INT NOT NULL,
                                                                 TotalQuestions INT NOT NULL,
                                                                 Correct INT NOT NULL,
                                                                 Incorrect INT NOT NULL,
                                                                 Score FLOAT NOT NULL,
                                                                 SessionDuration Time, 
                                                                 FOREIGN KEY (StackId) REFERENCES {TableNames.StudyStacksTable} (Id) ON DELETE CASCADE);
                                                     """;

    public static string CreateSessionFlashcardTable => $"""
                                                         IF OBJECT_ID(N'{TableNames.SessionFlashcardsTable}', N'U') IS NULL 
                                                         CREATE TABLE {TableNames.SessionFlashcardsTable} (
                                                         SessionId INT NOT NULL,
                                                         FlashcardId INT NOT NULL,
                                                         Question NVARCHAR(3000) NOT NULL,
                                                         Answer NVARCHAR(3000) NOT NULL,
                                                         UserAnswer NVARCHAR(3000) NOT NULL, 
                                                         IsCorrect BIT NOT NULL, 
                                                         DisplayPosition INT NOT NULL,
                                                         CONSTRAINT session_flashcards_pk PRIMARY KEY (SessionId, FlashcardId),
                                                         CONSTRAINT FK_session 
                                                            FOREIGN KEY (SessionId) REFERENCES {TableNames.StudySessionsTable}(Id));
                                                         """;
}