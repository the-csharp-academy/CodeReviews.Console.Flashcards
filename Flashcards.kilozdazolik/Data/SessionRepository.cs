using Dapper;
using Flashcards.kilozdazolik.Models;
using Microsoft.Data.SqlClient;

namespace Flashcards.kilozdazolik.Data;

public class SessionRepository
{
    public void InsertSession(Session session)
    {
        try
        {
            using (var conn = Database.GetConnection())
            {
                var sql = "INSERT INTO dbo.sessions (stack_id, date, score) VALUES (@StackId, @Date, @Score)";
                conn.Execute(sql, session);
            }
        }
        catch (SqlException e)
        {
            throw new Exception("Database operation failed", e);
        }
    }

    public List<Session> GetStudySessions()
    {
        try
        {
            using (var conn = Database.GetConnection())
            {
                var sql = @"SELECT session_id AS SessionId, 
                               stack_id AS StackId, 
                               date AS Date, 
                               score AS Score 
                        FROM dbo.sessions 
                        ORDER BY date DESC";
                var sessions = conn.Query<Session>(sql);
                return sessions.ToList();
            }
        }
        catch (SqlException e)
        {
            throw new Exception("Database operation failed", e);
        }
    }
}