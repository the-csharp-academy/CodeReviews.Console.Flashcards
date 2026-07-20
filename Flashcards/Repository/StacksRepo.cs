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
        // throw something tho
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

    public void Delete(long stackId)
    {
        throw new NotImplementedException();
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

    public void Update(long stackId, string newName)
    {
        throw new NotImplementedException();
    }
}