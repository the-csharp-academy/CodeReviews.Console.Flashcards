using Dapper;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace CodeReviews.Console.Flashcards.Tests;

[TestFixture]
[Category("Integration")]
[NonParallelizable]
public sealed class FlashcardsRepoTests
{
    private TestDatabase _database;
    private IFlashcardsRepo _repository;
    private long _stackId;

    [SetUp]
    public void SetUp()
    {
        _database = new TestDatabase();
        _database.Initialize();

        _repository = new FlashcardsRepo(_database.connectionFactory);

        _stackId = InsertStack("German");
    }

    [TearDown]
    public void TearDown() => _database.Dispose();

    private long InsertStack(string name)
    {
        using var connection = _database.OpenConnection();

        return connection.QuerySingle<long>(
            """
            INSERT INTO dbo.Stacks (Name)
            VALUES (@Name);

            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """,
            new { Name = name });
    }

    [Test]
    public void Add_WhenCardIsValid_InsertsFlashcard()
    {
        var card = new Flashcard
        {
            StackId = _stackId,
            Question = "Haus",
            Answer = "House"
        };

        _repository.Add(card);

        using var connection = _database.OpenConnection();

        FlashcardRecord saved =
            connection.QuerySingle<FlashcardRecord>(
                """
                SELECT * FROM dbo.Flashcards
                WHERE StackId = @StackId
                  AND Question = @Question;
                """,
                new
                {
                    StackId = _stackId,
                    Question = "Haus"
                });

        Assert.Multiple(() =>
        {
            Assert.That(saved.FlashcardId, Is.GreaterThan(0));
            Assert.That(saved.StackId, Is.EqualTo(_stackId));
            Assert.That(saved.Question, Is.EqualTo("Haus"));
            Assert.That(saved.Answer, Is.EqualTo("House"));
        });
    }

    [Test]
    public void GetAllByStackId_ReturnsOnlyCardsFromRequestedStack()
    {
        long secondStackId = InsertStack("Polish");

        InsertFlashcard(new FlashcardRecord
        {
            StackId = _stackId,
            Question = "Haus",
            Answer = "House"
        });
        InsertFlashcard(new FlashcardRecord
        {
            StackId = _stackId,
            Question = "Baum",
            Answer = "Tree"
        });
        InsertFlashcard(new FlashcardRecord
        {
            StackId = secondStackId,
            Question = "Dom",
            Answer = "House"
        });

        IReadOnlyList<Flashcard> result = _repository.GetAllByStackId(_stackId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));

            Assert.That(
                result.Select(card => card.Question),
                Is.EquivalentTo(new[] { "Haus", "Baum" }));

            Assert.That(
                result.All(card => card.StackId == _stackId),
                Is.True);
        });
    }

    [Test]
    public void GetAllByStackId_WhenStackHasNoCards_ReturnsEmptyList()
    {
        IReadOnlyList<Flashcard> result = _repository.GetAllByStackId(_stackId);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Update_WhenCardExists_ChangesQuestionAndAnswer()
    {
        long cardId = InsertFlashcard(new FlashcardRecord
        {
            StackId = _stackId,
            Question = "Old question",
            Answer = "Old answer"
        });

        var updatedCard = new Flashcard
        {
            FlashcardId = cardId,
            StackId = _stackId,
            Question = "New question",
            Answer = "New answer"
        };

        _repository.Update(updatedCard);

        using var connection = _database.OpenConnection();

        FlashcardRecord saved =
            connection.QuerySingle<FlashcardRecord>(
                """
                SELECT * FROM dbo.Flashcards
                WHERE FlashcardId = @FlashcardId;
                """,
                new { FlashcardId = cardId });

        Assert.Multiple(() =>
        {
            Assert.That(saved.Question, Is.EqualTo("New question"));
            Assert.That(saved.Answer, Is.EqualTo("New answer"));
        });
    }

    [Test]
    public void Delete_WhenCardExists_RemovesFlashcard()
    {
        long cardId = InsertFlashcard(new FlashcardRecord
        {
            StackId = _stackId,
            Question = "Haus",
            Answer = "House"
        });

        _repository.Delete(cardId);

        using var connection = _database.OpenConnection();

        int remaining = connection.QuerySingle<int>(
            """
            SELECT COUNT(*)
            FROM dbo.Flashcards
            WHERE FlashcardId = @FlashcardId;
            """,
            new { FlashcardId = cardId });

        Assert.That(remaining, Is.Zero);
    }

    [Test]
    public void Add_WhenStackDoesNotExist_ThrowsSqlException()
    {
        var card = new Flashcard
        {
            StackId = long.MaxValue,
            Question = "Question",
            Answer = "Answer"
        };

        Assert.Throws<SqlException>(() => _repository.Add(card));
    }

    [TestCase("", "Answer")]
    [TestCase(" ", "Answer")]
    [TestCase("Question", "")]
    [TestCase("Question", " ")]
    public void Add_WhenContentIsInvalid_ThrowsArgumentException(string question, string answer)
    {
        var card = new Flashcard
        {
            StackId = _stackId,
            Question = question,
            Answer = answer
        };

        Assert.Throws<ArgumentException>(() => _repository.Add(card));
    }

    private long InsertFlashcard(FlashcardRecord record)
    {
        using var connection = _database.OpenConnection();

        return connection.QuerySingle<long>(
            """
            INSERT INTO dbo.Flashcards
                (StackId, Question, Answer)
            VALUES
                (@StackId, @Question, @Answer);

            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """,
            new
            {
                StackId = record.StackId,
                Question = record.Question,
                Answer = record.Answer
            });
    }

    private sealed class FlashcardRecord
    {
        public long FlashcardId { get; set; }
        public long StackId { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }
}