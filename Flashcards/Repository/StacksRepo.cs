using Dapper;

namespace CodeReviews.Console.Flashcards;

public sealed class StacksRepo : IStacksRepo
{
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public StacksRepo(IDatabaseConnectionFactory _cf) => _connectionFactory = _cf;
    public void Add(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException();

        const string sql = @"
            INSERT INTO dbo.Stacks (Name)
            VALUES (@Name);
        ";
        using (var connection = _connectionFactory.CreateDatabaseConnection())
        {
            connection.Open();
            connection.Execute(sql, new { Name = name });
        }
    }

    public void Delete(int stackId)
    {
        const string sql = @"
            DELETE FROM dbo.Stacks
            WHERE StackId = @StackId;
        ";
        using (var connection = _connectionFactory.CreateDatabaseConnection())
        {
            connection.Open();
            connection.Execute(sql, new { StackId = stackId });
        }
    }

    public List<CardStack> GetAll()
    {
        List<CardStack> stacks = new();

        const string sql = @"SELECT * FROM dbo.Stacks;";
        using (var connection = _connectionFactory.CreateDatabaseConnection())
        {
            connection.Open();
            IEnumerable<CardStack> rows = connection.Query<CardStack>(sql);
            foreach (var item in rows)
            {
                stacks.Add(new CardStack
                {
                    StackId = item.StackId,
                    Name = item.Name
                });
            }
        }
        return stacks;
    }

    public CardStack? GetStackByName(string name)
    {
        const string sql = @"
            SELECT * FROM dbo.Stacks
            WHERE Name = @Name
        ";
        using (var connection = _connectionFactory.CreateDatabaseConnection())
        {
            connection.Open();
            return connection.QuerySingleOrDefault<CardStack>(sql, new { Name = name });
        }
    }

    public void Update(int stackId, string newName)
    {
        // this method is questionable
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentNullException();

        const string sql = @"
            UPDATE dbo.Stacks
            SET Name = @Name
            WHERE StackId = @StackId;
        ";
        using (var connection = _connectionFactory.CreateDatabaseConnection())
        {
            connection.Open();
            connection.Execute(sql, new { StackId = stackId, Name = newName });
        }
    }
}