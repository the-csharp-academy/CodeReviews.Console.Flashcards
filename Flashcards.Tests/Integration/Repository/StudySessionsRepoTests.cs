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
            Score = 8,
            TotalQuestions = 10
        };

        DateTime beforeInsert = DateTime.UtcNow;
        _repository.Add(session);
        DateTime afterInsert = DateTime.UtcNow;

        using (var connection = _database.OpenConnection())
        {
            const string sql = @"
                SELECT
                    SessionId,
                    StackId,
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
                Assert.That(saved.Score, Is.EqualTo(8));
                Assert.That(saved.TotalQuestions, Is.EqualTo(10));
                Assert.That(saved.CompletedAt, Is.InRange(beforeInsert.AddSeconds(-1), afterInsert.AddSeconds(1)));
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
        InsertSession(2, 4, new DateTime(2026, 7, 20, 10, 0, 0));
        InsertSession(4, 5, new DateTime(2026, 7, 24, 10, 0, 0));

        IReadOnlyList<StudySessionDTO> result = _repository.GetAll();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));

            Assert.That(result[0].StackName, Is.EqualTo("German"));
            Assert.That(result[0].Score, Is.EqualTo(4));
            Assert.That(result[0].TotalQuestions, Is.EqualTo(5));
            Assert.That(result[0].Percentage, Is.EqualTo(80));

            Assert.That(result[1].StackName, Is.EqualTo("German"));
            Assert.That(result[1].Score, Is.EqualTo(2));
            Assert.That(result[1].TotalQuestions, Is.EqualTo(4));
        });
    }

    [Test]
    public void RenameStack_ChangesDisplayedStackNameInHistory()
    {
        _repository.Add(
            new StudySession
            {
                StackId = _stackId,
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
        Assert.That(saved.StackName, Is.EqualTo("German B2"));
    }

    [Test]
    public void Add_WhenStackDoesNotExist_ThrowsSqlException()
    {
        var session = new StudySession
        {
            StackId = int.MaxValue,
            Score = 1,
            TotalQuestions = 1
        };

        Assert.Throws<SqlException>(() => _repository.Add(session));
    }

    [TestCase(-1, 5)]
    [TestCase(6, 5)]
    public void Add_WhenScoreIsInvalid_ThrowsArgumentOutOfRangeException(int score, int totalQuestions)
    {
        var session = new StudySession
        {
            StackId = _stackId,
            Score = score,
            TotalQuestions = totalQuestions
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => _repository.Add(session));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Add_WhenTotalQuestionsIsNotPositive_ThrowsArgumentOutOfRangeException(
    int totalQuestions)
    {
        var session = new StudySession
        {
            StackId = _stackId,
            Score = 0,
            TotalQuestions = totalQuestions
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => _repository.Add(session));
    }

    [Test]
    public void Add_WhenSessionIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => _repository.Add(null!));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Add_WhenStackIdIsNotPositive_ThrowsArgumentOutOfRangeException(
    int stackId)
    {
        var session = new StudySession
        {
            StackId = stackId,
            Score = 1,
            TotalQuestions = 1
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => _repository.Add(session));
    }

    [Test]
    public void GetAll_WhenSessionsHaveSameCompletionTime_ReturnsLatestInsertedFirst()
    {
        DateTime completedAt = new(2026, 7, 24, 10, 0, 0);

        InsertSession(1, 4, completedAt);
        InsertSession(3, 4, completedAt);

        IReadOnlyList<StudySessionDTO> result = _repository.GetAll();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Score, Is.EqualTo(3));
            Assert.That(result[1].Score, Is.EqualTo(1));
        });
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

    private void InsertSession(int score, int totalQuestions, DateTime completedAt)
    {
        using var connection = _database.OpenConnection();
        connection.Execute(
            """
            INSERT INTO dbo.StudySessions
            (
                StackId,
                Score,
                TotalQuestions,
                CompletedAt
            )
            VALUES
            (
                @StackId,
                @Score,
                @TotalQuestions,
                @CompletedAt
            );
            """,
            new
            {
                StackId = _stackId,
                Score = score,
                TotalQuestions = totalQuestions,
                CompletedAt = completedAt
            });
    }

    private sealed class StudySessionRecord
    {
        public int SessionId { get; set; }
        public int StackId { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}