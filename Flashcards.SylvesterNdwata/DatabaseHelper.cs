using Dapper;
using Microsoft.Data.SqlClient;

namespace silvermax.FlashCards;

internal class DatabaseHelper
{
    private readonly DbConfig dbConfig = new();

    public SqlConnection GetConnection() => new SqlConnection(dbConfig.ConnectionString);

    public int GetStackId(string stackName)
    {
        using var connection = GetConnection();

        return connection.QuerySingle<int>("SELECT Id FROM Stacks WHERE StackName = @StackName", new { StackName = stackName });
    }

    public bool StackExists(int stackId)
    {
        using var connection = GetConnection();

        return connection.ExecuteScalar<bool>("SELECT COUNT(1) FROM Stacks WHERE Id = @Id", new { Id = stackId });
    }

    public bool FlashCardExists(string word)
    {
        using var connection = GetConnection();
        return connection.ExecuteScalar<bool>("SELECT COUNT(1) FROM Flashcards WHERE Word = @Word", new { Word = word });
    }
}
