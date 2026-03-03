using System.Diagnostics;
using Dapper;
using Flashcards._0lcm.Logging;
using Flashcards._0lcm.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Flashcards._0lcm.CRUDController;

internal class LocalDbController
{
    private static readonly ILogger Logger = AppLogger.CreateLogger<LocalDbController>();

    private static readonly string LocalDbName =
        Program.Configuration["Configurations:LocalDbInstanceName"] ?? "flashcards.0lcm.LocalDb";

    //------- Connection Factory -------
    private static SqlConnection CreateOpenConnection()
    {
        var connectionString = Program.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        var connection = new SqlConnection(connectionString);
        connection.Open();
        return connection;
    }

    //------- Execution Helpers -------
    private static void Execute(string sql, object? parameters = null)
    {
        using var connection = CreateOpenConnection();
        connection.Execute(sql, parameters);
    }

    //------- Initialize Database -------
    internal static void CreateLocalDbInstance()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "sqllocaldb",
                Arguments = $"create {LocalDbName}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            process?.WaitForExit();
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Could not start Local Db instance.");
            throw;
        }
    }
    
    internal static void StartLocalDb()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "sqllocaldb",
                Arguments = $"start {LocalDbName}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            process?.WaitForExit();
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Could not start Local Db instance.");
            throw;
        }
    }

    internal static void CreateDatabase()
    {
        var masterConnectionString = Program.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(masterConnectionString))
            throw new InvalidOperationException("Connection stirng 'DefaultConnection' not found.");

        var builder = new SqlConnectionStringBuilder(masterConnectionString)
        {
            InitialCatalog = "master"
        };

        const string createDatabaseCommand = @"
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FlashcardsDatabase')
                BEGIN
                    CREATE DATABASE FlashcardsDatabase
                END";

        try
        {
            using var connection = new SqlConnection(builder.ConnectionString);
            connection.Open();
            connection.Execute(createDatabaseCommand);
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Error creating database.");
            throw;
        }
    }

    internal static void CreateTables()
    {
        const string createStackTableCommand = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stacks')
                BEGIN
                    CREATE TABLE Stacks (
                    StackId INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(100) UNIQUE NOT NULL
                )
                END";

        const string createFlashcardTableCommand = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Flashcards')
                BEGIN
                    CREATE TABLE Flashcards (
                    CardId INT PRIMARY KEY IDENTITY (1,1),
                    Name NVARCHAR(100) NOT NULL,
                    Value NVARCHAR(200) NOT NULL,
                    StackId INT,
                    FOREIGN KEY (StackId) REFERENCES Stacks(StackId)
                )
                END";

        const string createStudySessionTableCommand = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StudySessions')
                BEGIN
                    CREATE TABLE StudySessions (
                    SessionId INT PRIMARY KEY IDENTITY (1,1),
                    StudyCount INT,
                    Date DATE NOT NULL,
                    StackId INT,
                    FOREIGN KEY (StackId) REFERENCES Stacks(StackId)
                )
                END";

        try
        {
            Execute(createStackTableCommand);
            Execute(createFlashcardTableCommand);
            Execute(createStudySessionTableCommand);
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Error creating database table.");
            throw;
        }
    }

    //------- Stack CRUD -------
    internal static void CreateStack(Stack stack)
    {
        const string createStackCommand = @"
                INSERT INTO Stacks (Name)
                VALUES (@Name);";

        try
        {
            using var connection = CreateOpenConnection();
            var stackId = connection.ExecuteScalar<int>(createStackCommand, new
            {
                stack.Name
            });

            stack.StackId = stackId;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Could not insert new stack into table Stacks");
            throw;
        }
    }

    internal static void UpdateStack(Stack stack)
    {
        const string updateCommand = @"
                UPDATE Stacks
                SET Name = @Name
                WHERE StackId = @StackId;";

        try
        {
            Execute(updateCommand, new
            {
                stack.StackId,
                stack.Name
            });
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Could not update stack.");
            throw;
        }
    }

    internal static void DeleteStackAndDependents(Stack stack)
    {
        const string deleteCommand = "DELETE FROM Stacks WHERE StackId = @StackId";

        try
        {
            DeleteAllFlashcardsForStack(stack.StackId);
            DeleteAllSessionsForStack(stack.StackId);
            Execute(deleteCommand, new
            {
                stack.StackId
            });
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Could not delete stack from table Stacks where StackId");
            throw;
        }
    }

    //------- Flashcard CRUD -------
    internal static void CreateFlashcard(Flashcard flashcard)
    {
        const string createFlashcardCommand = @"
                INSERT INTO Flashcards (Name, Value, StackId)
                VALUES (@Name, @Value, @StackId);";

        try
        {
            using var connection = CreateOpenConnection();
            var cardId = connection.ExecuteScalar<int>(createFlashcardCommand, new
            {
                flashcard.Name,
                flashcard.Value,
                flashcard.StackId
            });

            flashcard.CardId = cardId;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Could not insert new flashcard into table Flashcards.");
            throw;
        }
    }

    internal static void UpdateFlashcard(Flashcard flashcard)
    {
        const string updateCommand = @"
                UPDATE Flashcards
                SET Name = @Name, Value = @Value
                WHERE CardId = @CardId;";

        try
        {
            Execute(updateCommand, new
            {
                flashcard.Name,
                flashcard.Value,
                flashcard.CardId
            });
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Could not update flashcard with name and value where CardId in Flashcards table");
            throw;
        }
    }

    internal static void DeleteFlashcard(Flashcard flashcard)
    {
        const string deleteCommand = @"
                DELETE FROM Flashcards WHERE CardId = @CardId;";

        try
        {
            Execute(deleteCommand, new
            {
                flashcard.CardId
            });
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Could not delete flashcard in table Flashcards");
            throw;
        }
    }

    private static void DeleteAllFlashcardsForStack(int stackId)
    {
        const string deleteCommand = "DELETE FROM Flashcards WHERE StackId = @StackId";

        try
        {
            Execute(deleteCommand, new
            {
                StackId = stackId
            });
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Could not delete all flashcards in table Flashcards for StackId");
            throw;
        }
    }

    //------- Study Session CRUD -------
    internal static void InsertSession(StudySession studySession)
    {
        const string insertCommand = @"
                    INSERT INTO StudySessions (StudyCount, StackId, Date)
                    VALUES (@StudyCount, @StackId, @Date);";

        try
        {
            using var connection = CreateOpenConnection();
            var sessionId = connection.ExecuteScalar<int>(insertCommand, new
            {
                studySession.StudyCount,
                studySession.Date,
                studySession.StackId
            });

            studySession.SessionId = sessionId;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Could not insert session into table Sessions");
            throw;
        }
    }

    private static void DeleteAllSessionsForStack(int stackId)
    {
        const string deleteCommand = "DELETE FROM StudySessions WHERE StackId = @StackId";

        try
        {
            Execute(deleteCommand, new
            {
                StackId = stackId
            });
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Could not delete all sessions in table Sessions");
            throw;
        }
    }

    //------- Queries -------
    internal static List<Stack> GetStacks()
    {
        const string query = "SELECT * FROM Stacks";

        using var connection = CreateOpenConnection();
        var results = connection.Query<Stack>(query).ToList();

        return results.Select(row => new Stack
        {
            StackId = row.StackId,
            Name = row.Name
        }).ToList();
    }

    internal static List<Flashcard> GetFlashcardsForStack(int stackId)
    {
        const string query = "SELECT * FROM Flashcards WHERE StackId = @StackId";

        using var connection = CreateOpenConnection();
        var results = connection.Query<Flashcard>(query, new
        {
            stackId
        }).ToList();

        return results.Select(row => new Flashcard
        {
            CardId = row.CardId,
            Name = row.Name,
            Value = row.Value,
            StackId = row.StackId
        }).ToList();
    }

    internal static List<StudySession> GetSessionsForStack(int stackId)
    {
        const string query = "SELECT * FROM StudySessions WHERE StackId = @StackId";

        using var connection = CreateOpenConnection();
        var results = connection.Query<StudySession>(query, new
        {
            StackId = stackId
        }).ToList();

        return results.Select(row => new StudySession
        {
            SessionId = row.SessionId,
            StudyCount = row.StudyCount,
            StackId = row.StackId
        }).ToList();
    }

    internal static int GetFlashcardCountForStack(int stackId)
    {
        const string query = "SELECT COUNT(*) FROM Flashcards WHERE StackId = @StackId";

        using var connection = CreateOpenConnection();
        return connection.ExecuteScalar<int>(query, new { stackId });
    }
}