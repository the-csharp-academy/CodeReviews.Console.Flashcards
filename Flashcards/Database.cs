using Microsoft.Data.SqlClient;

namespace Flashcards;

public sealed class Database(string connectionString)
{
    public string ConnectionString { get; } = connectionString;

    public async Task InitializeAsync()
    {
        var builder = new SqlConnectionStringBuilder(ConnectionString);
        var databaseName = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException("The connection string must specify a database.");

        var masterBuilder = new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = "master" };
        await using (var master = new SqlConnection(masterBuilder.ConnectionString))
        {
            await master.OpenAsync();
            // Database identifiers cannot be SQL parameters. QuoteIdentifier safely
            // escapes the name while the existence check remains parameterized.
            var quotedDatabaseName = new SqlCommandBuilder().QuoteIdentifier(databaseName);
            await using var createDatabase = new SqlCommand(
                $"IF DB_ID(@name) IS NULL CREATE DATABASE {quotedDatabaseName}", master);
            createDatabase.Parameters.AddWithValue("@name", databaseName);
            await createDatabase.ExecuteNonQueryAsync();
        }

        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        const string schema = """
            IF OBJECT_ID('dbo.Stacks', 'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Stacks (
                    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Stacks PRIMARY KEY,
                    Name nvarchar(100) NOT NULL CONSTRAINT UQ_Stacks_Name UNIQUE
                );
            END;

            IF OBJECT_ID('dbo.Flashcards', 'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Flashcards (
                    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Flashcards PRIMARY KEY,
                    StackId int NOT NULL,
                    Front nvarchar(500) NOT NULL,
                    Back nvarchar(500) NOT NULL,
                    CONSTRAINT FK_Flashcards_Stacks FOREIGN KEY (StackId)
                        REFERENCES dbo.Stacks(Id) ON DELETE CASCADE
                );
                CREATE INDEX IX_Flashcards_StackId ON dbo.Flashcards(StackId);
            END;

            IF OBJECT_ID('dbo.StudySessions', 'U') IS NULL
            BEGIN
                CREATE TABLE dbo.StudySessions (
                    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_StudySessions PRIMARY KEY,
                    StackId int NOT NULL,
                    StudiedAt datetime2 NOT NULL,
                    Score int NOT NULL,
                    TotalCards int NOT NULL,
                    CONSTRAINT CK_StudySessions_Score CHECK (Score >= 0 AND TotalCards >= 0 AND Score <= TotalCards),
                    CONSTRAINT FK_StudySessions_Stacks FOREIGN KEY (StackId)
                        REFERENCES dbo.Stacks(Id) ON DELETE CASCADE
                );
                CREATE INDEX IX_StudySessions_StackId_StudiedAt
                    ON dbo.StudySessions(StackId, StudiedAt);
            END;
            """;
        await using var command = new SqlCommand(schema, connection);
        await command.ExecuteNonQueryAsync();
    }
}
