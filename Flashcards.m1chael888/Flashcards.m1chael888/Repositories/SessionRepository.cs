using Flashcards.m1chael888.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Flashcards.m1chael888.Repositories;

public interface ISessionRepository
{
    void Create(SessionModel model);
    List<SessionModel> Read();
}
public class SessionRepository : ISessionRepository
{
    private readonly string _connectionString;
    public SessionRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Create(SessionModel model)
    {
        var sql = "INSERT INTO Sessions (Date, Score, StackId) VALUES (@Date, @Score, @StackId)";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Execute(sql, new
            {
                Date = model.Date,
                Score = model.Score,
                StackId = model.StackId
            });
        }
    }

    public List<SessionModel> Read()
    {
        var sessions = new List<SessionModel>();
        var sql = "SELECT * FROM Sessions";

        using (var connection = new SqlConnection(_connectionString))
        {
            sessions = connection.Query<SessionModel>(sql).ToList();
        }
        return sessions;
    }
}
