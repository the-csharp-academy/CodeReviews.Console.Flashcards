using Dapper;
using Flashcards.m1chael888.Models;
using Microsoft.Data.SqlClient;

namespace Flashcards.m1chael888.Repositories;

public interface IStackRepository
{
    void Create(string stackName);
    List<StackModel> Read();
    void Update(StackModel stack);
    void Delete(int stackId);
}
public class StackRepository : IStackRepository
{
    private readonly string _connectionString;
    public StackRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Create(string stackName)
    {
        var sql = @"INSERT INTO Stacks (Name) VALUES (@Name)";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Execute(sql, new
            {
                Name = stackName
            });
        }
    }

    public List<StackModel> Read()
    {
        var stacks = new List<StackModel>();
        var sql = @"SELECT * FROM Stacks";

        using (var connection = new SqlConnection(_connectionString))
        {
            stacks = connection.Query<StackModel>(sql).ToList();
        }
        return stacks;
    }

    public void Update(StackModel stack)
    {
        var sql = @"UPDATE Stacks SET Name = @StackName WHERE StackId = @StackId";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Execute(sql, new
            {
                StackName = stack.Name,
                StackId = stack.StackId
            });
        }
    }

    public void Delete(int stackId)
    {
        var sql = @"DELETE FROM Sessions WHERE StackId = @StackId
                        DELETE FROM Cards WHERE StackId = @StackId
                        DELETE FROM Stacks WHERE StackId = @StackId";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Execute(sql, new
            {
                StackId = stackId
            });
        }
    }
}
