using Dapper;
using Microsoft.Data.SqlClient;

namespace CodeReviews.Console.Flashcards.Tests;

[TestFixture]
[Category("Integration")]
[NonParallelizable]
public sealed class StudySessionsRepoTests
{
    private TestDatabase _database;
    private IStudySessionsRepo _repository;
    private int _stackId;

    [SetUp]
    public void SetUp()
    {
        _database = new TestDatabase();
        _database.Initialize();

        _repository = new StudySessionsRepo(_database.connectionFactory);

        _stackId = InsertStack("German");
    }

    [TearDown]
    public void TearDown() => _database.Dispose();

    [Test]
    public void Add_WhenSessionIsValid_InsertsSession()
    {
        var session = new StudySession
        {
            StackId = _stackId,
            StackNameSnapshot = "German",
            Score = 8,
            TotalQuestions = 10
        };

        _repository.Add(session);

        using (var connection = _database.OpenConnection())
        {
            const string sql = @"
                SELECT
                    SessionId,
                    StackId,
                    StackNameSnapshot,
                    Score,
                    TotalQuestions,
                    CompletedAt
                FROM dbo.StudySessions;
            ";
            StudySessionRecord saved = connection.QuerySingle<StudySessionRecord>(sql);

            Assert.Multiple(() =>
            {
                Assert.That(saved.SessionId, Is.GreaterThan(0));
                Assert.That(saved.StackId, Is.EqualTo(_stackId));
                Assert.That(saved.StackNameSnapshot, Is.EqualTo("German"));
                Assert.That(saved.Score, Is.EqualTo(8));
                Assert.That(saved.TotalQuestions, Is.EqualTo(10));
                Assert.That(saved.CompletedAt, Is.Not.EqualTo(default(DateTime)));
            });
        }

    }

    [Test]
    public void GetAll_WhenNoSessionsExist_ReturnsEmptyList()
    {
        IReadOnlyList<StudySessionDTO> result = _repository.GetAll();
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetAll_WhenSessionsExist_ReturnsNewestFirst()
    {
        InsertSession("German old", 2, 4, new DateTime(2026, 7, 20, 10, 0, 0));
        InsertSession("German new", 4, 5, new DateTime(2026, 7, 24, 10, 0, 0));

        IReadOnlyList<StudySessionDTO> result = _repository.GetAll();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].StackName, Is.EqualTo("German new"));
            Assert.That(result[1].StackName, Is.EqualTo("German old"));
            Assert.That(result[0].Percentage, Is.EqualTo(80));
        });
    }

    [Test]
    public void DeleteStack_PreservesSessionAndSetsStackIdToNull()
    {
        _repository.Add(
            new StudySession
            {
                StackId = _stackId,
                StackNameSnapshot = "German",
                Score = 7,
                TotalQuestions = 10
            });

        using (var connection = _database.OpenConnection())
        {
            connection.Execute(
                """
                DELETE FROM dbo.Stacks
                WHERE StackId = @StackId;
                """,
                new { StackId = _stackId });
        }

        using (var connection = _database.OpenConnection())
        {
            StudySessionRecord saved =
                connection.QuerySingle<StudySessionRecord>(
                    """
                    SELECT
                        SessionId,
                        StackId,
                        StackNameSnapshot,
                        Score,
                        TotalQuestions,
                        CompletedAt
                    FROM dbo.StudySessions;
                    """);

            Assert.Multiple(() =>
            {
                Assert.That(saved.StackId, Is.Null);
                Assert.That(saved.StackNameSnapshot, Is.EqualTo("German"));
            });
        }

        IReadOnlyList<StudySessionDTO> history = _repository.GetAll();

        Assert.Multiple(() =>
        {
            Assert.That(history, Has.Count.EqualTo(1));
            Assert.That(history[0].StackName, Is.EqualTo("German"));
            Assert.That(history[0].Score, Is.EqualTo(7));
        });
    }

    [Test]
    public void RenameStack_DoesNotChangeHistoricalSnapshot()
    {
        _repository.Add(
            new StudySession
            {
                StackId = _stackId,
                StackNameSnapshot = "German",
                Score = 5,
                TotalQuestions = 6
            });

        using (var connection = _database.OpenConnection())
        {
            connection.Execute(
                """
                UPDATE dbo.Stacks
                SET Name = @NewName
                WHERE StackId = @StackId;
                """,
                new
                {
                    NewName = "German B2",
                    StackId = _stackId
                });
        }

        StudySessionDTO saved = _repository.GetAll().Single();
        Assert.That(saved.StackName, Is.EqualTo("German"));
    }

    [Test]
    public void Add_WhenStackDoesNotExist_ThrowsSqlException()
    {
        var session = new StudySession
        {
            StackId = int.MaxValue,
            StackNameSnapshot = "Missing",
            Score = 1,
            TotalQuestions = 1
        };

        Assert.Throws<SqlException>(() => _repository.Add(session));
    }

    [Test]
    public void Add_WhenStackIdIsNull_ThrowsArgumentOutOfRangeException()
    {
        var session = new StudySession
        {
            StackId = null,
            StackNameSnapshot = "German",
            Score = 1,
            TotalQuestions = 1
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => _repository.Add(session));
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("   ")]
    public void Add_WhenSnapshotIsBlank_ThrowsArgumentException(string snapshot)
    {
        var session = new StudySession
        {
            StackId = _stackId,
            StackNameSnapshot = snapshot,
            Score = 1,
            TotalQuestions = 1
        };

        Assert.Throws<ArgumentException>(() => _repository.Add(session));
    }

    [TestCase(-1, 5)]
    [TestCase(6, 5)]
    public void Add_WhenScoreIsInvalid_ThrowsArgumentOutOfRangeException(int score, int totalQuestions)
    {
        var session = new StudySession
        {
            StackId = _stackId,
            StackNameSnapshot = "German",
            Score = score,
            TotalQuestions = totalQuestions
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => _repository.Add(session));
    }

    [Test]
    public void Add_WhenTotalQuestionsIsZero_ThrowsArgumentOutOfRangeException()
    {
        var session = new StudySession
        {
            StackId = _stackId,
            StackNameSnapshot = "German",
            Score = 0,
            TotalQuestions = 0
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => _repository.Add(session));
    }

    private int InsertStack(string name)
    {
        using var connection = _database.OpenConnection();
        return connection.QuerySingle<int>(
            """
            INSERT INTO dbo.Stacks (Name)
            VALUES (@Name);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """,
            new { Name = name });
    }

    private void InsertSession(string stackNameSnapshot, int score, int totalQuestions, DateTime completedAt)
    {
        using var connection = _database.OpenConnection();
        connection.Execute(
            """
            INSERT INTO dbo.StudySessions
            (
                StackId,
                StackNameSnapshot,
                Score,
                TotalQuestions,
                CompletedAt
            )
            VALUES
            (
                @StackId,
                @StackNameSnapshot,
                @Score,
                @TotalQuestions,
                @CompletedAt
            );
            """,
            new
            {
                StackId = _stackId,
                StackNameSnapshot = stackNameSnapshot,
                Score = score,
                TotalQuestions = totalQuestions,
                CompletedAt = completedAt
            });
    }

    private sealed class StudySessionRecord
    {
        public int SessionId { get; set; }
        public int? StackId { get; set; }
        public string StackNameSnapshot { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}