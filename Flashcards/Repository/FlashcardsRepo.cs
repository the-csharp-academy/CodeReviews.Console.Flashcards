using Dapper;

namespace CodeReviews.Console.Flashcards;

public sealed class FlashcardsRepo : IFlashcardsRepo
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    public FlashcardsRepo(IDatabaseConnectionFactory cf)
        => _connectionFactory = cf ??
            throw new ArgumentNullException(nameof(cf));


    public void Add(Flashcard card)
    {
        if (card == null ||
            string.IsNullOrWhiteSpace(card.Question) ||
            string.IsNullOrWhiteSpace(card.Answer))
            throw new ArgumentException();

        const string sql = @"
            INSERT INTO dbo.Flashcards
            (Question, Answer, StackId)
            VALUES (@Question, @Answer, @StackId);
        ";
        using (var connection = _connectionFactory.CreateDatabaseConnection())
        {
            connection.Open();
            connection.Execute(sql, new
            {
                Question = card.Question,
                Answer = card.Answer,
                StackId = card.StackId
            });
        }
    }

    public void Delete(int cardId)
    {
        const string sql = @"
            DELETE FROM dbo.Flashcards
            WHERE FlashcardId = @FlashcardId;
        ";
        using (var connection = _connectionFactory.CreateDatabaseConnection())
        {
            connection.Open();
            connection.Execute(sql, new { FlashcardId = cardId });
        }
    }

    public IReadOnlyList<Flashcard> GetAllByStackId(int stackId)
    {
        List<Flashcard> cards = new();

        const string sql = @"
            SELECT * FROM dbo.Flashcards
            WHERE StackId = @StackId
            ORDER BY FlashcardId;
        ";
        using (var connection = _connectionFactory.CreateDatabaseConnection())
        {
            connection.Open();
            IEnumerable<Flashcard> rows = connection.Query<Flashcard>(sql, new
            {
                StackId = stackId
            });

            foreach (var item in rows)
            {
                cards.Add(new Flashcard
                {
                    FlashcardId = item.FlashcardId,
                    StackId = item.StackId,
                    Question = item.Question,
                    Answer = item.Answer
                });
            }
        }
        return cards.AsReadOnly();
    }

    public void Update(Flashcard card)
    {
        if (card == null ||
            string.IsNullOrWhiteSpace(card.Question) ||
            string.IsNullOrWhiteSpace(card.Answer))
            throw new ArgumentException();

        const string sql = @"
            UPDATE dbo.Flashcards
            SET Question = @Question, 
                Answer = @Answer
            WHERE FlashcardId = @FlashcardId;
        ";
        using (var connection = _connectionFactory.CreateDatabaseConnection())
        {
            connection.Open();
            connection.Execute(sql, new
            {
                FlashcardId = card.FlashcardId,
                Question = card.Question,
                Answer = card.Answer,
            });
        }
    }
}