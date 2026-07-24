using Dapper;
namespace CodeReviews.Console.Flashcards;

public sealed class StudySessionsRepo : IStudySessionsRepo
{
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public StudySessionsRepo(IDatabaseConnectionFactory cf)
        => _connectionFactory = cf ??
            throw new ArgumentNullException(nameof(cf));

    public void Add(StudySession session)
    {
        ValidateSession(session);

        const string sql = @"
            INSERT INTO dbo.StudySessions(StackId, Score, TotalQuestions)
            VALUES(@StackId, @Score, @TotalQuestions);
        ";

        using (var connection = _connectionFactory.CreateDatabaseConnection())
        {
            connection.Open();
            connection.Execute(sql, session);
        }
    }

    public IReadOnlyList<StudySessionDTO> GetAll()
    {
        const string sql = @"
            SELECT
                s.Name AS StackName,
                ss.Score,
                ss.TotalQuestions,
                ss.CompletedAt
            FROM dbo.StudySessions ss
            INNER JOIN dbo.Stacks s
                ON s.StackId = ss.StackId
            ORDER BY ss.CompletedAt DESC;
        ";
        using (var connection = _connectionFactory.CreateDatabaseConnection())
        {
            connection.Open();
            return connection.Query<StudySessionDTO>(sql).ToList().AsReadOnly();
        }
    }

    private static void ValidateSession(StudySession session)
    {
        if (session == null)
            throw new ArgumentNullException();

        if (session.StackId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(session.StackId));
        }

        if (session.TotalQuestions <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(session.TotalQuestions));
        }

        if (session.Score < 0 ||
            session.Score > session.TotalQuestions)
        {
            throw new ArgumentOutOfRangeException(
                nameof(session.Score));
        }
    }
}