namespace Flashcards.Tests;

using CodeReviews.Console.Flashcards.Tests;
using Dapper;
using Microsoft.Data.SqlClient;

[TestFixture]
[NonParallelizable]
[Category("Integration")]
public sealed class DatabaseInitializerTests
{
    private TestDatabase _db;

    [SetUp]
    public void Setup() => _db = new();

    [TearDown]
    public void TearDown() => _db.Dispose();

    [Test]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesRequiredTables()
    {
        _db.Initialize();
        // For now there are 2 tables
        using (var conn = _db.OpenConnection())
        {
            const string tablesQuery = @"
                SELECT name
                FROM sys.tables
                WHERE schema_id = SCHEMA_ID(N'dbo')
                AND name IN (N'Stacks', N'Flashcards');
            ";

            string[] tableNames = conn.Query<string>(tablesQuery).ToArray();

            Assert.That(tableNames, Is.EquivalentTo(new[] { "Stacks", "Flashcards" }));
        }
    }

    [Test]
    public void Initialize_WhenCalledTwice_PreservesExistingData()
    {
        _db.Initialize();

        using (var connection = _db.OpenConnection())
        {
            const string sql = @"
                INSERT INTO dbo.Stacks (Name)
                VALUES (@Name);
            ";
            connection.Execute(sql, new { Name = "Polish" });
        }

        Assert.DoesNotThrow(() => _db.Initialize());

        using (var verificationConnection = _db.OpenConnection())
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM dbo.Stacks
                WHERE Name = @Name;
            ";

            int count = verificationConnection.QuerySingle<int>(sql, new { Name = "Polish" });

            Assert.That(count, Is.EqualTo(1));
        }
    }

    [Test]
    public void Stacks_WhenNameIsDuplicated_RejectsSecondStack()
    {
        _db.Initialize();

        using (var connection = _db.OpenConnection())
        {
            const string sql = @"
                INSERT INTO dbo.Stacks (Name)
                VALUES (@Name);
            ";
            connection.Execute(sql, new { Name = "German" });

            Assert.Throws<SqlException>(()
                => connection.Execute(sql, new { Name = "German" }));
        }
    }

    [Test]
    public void Stacks_WhenDeleted_DeletesRelatedFlashcards()
    {
        _db.Initialize();

        using var connection = _db.OpenConnection();

        string sql = @"
            INSERT INTO dbo.Stacks (Name)
            VALUES (@Name);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
        ";

        int stackId = connection.QuerySingle<int>(sql, new { Name = "SQL" });

        sql = @"
            INSERT INTO dbo.Flashcards(StackId, Question, Answer)
            VALUES (@StackId, @Question, @Answer);
        ";

        connection.Execute(sql,
            new
            {
                StackId = stackId,
                Question = "What does SQL stand for?",
                Answer = "Structured Query Language"
            });

        sql = @"
            DELETE FROM dbo.Stacks
            WHERE StackId = @StackId;
        ";
        connection.Execute(sql, new { StackId = stackId });

        sql = @"
            SELECT COUNT(*)
            FROM dbo.Flashcards
            WHERE StackId = @StackId;
        ";

        int remainingFlashcards = connection.QuerySingle<int>(sql, new { StackId = stackId });

        Assert.That(remainingFlashcards, Is.Zero);
    }
}