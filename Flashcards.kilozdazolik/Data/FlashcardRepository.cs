using Dapper;
using Flashcards.kilozdazolik.Models;
using Microsoft.Data.SqlClient;

namespace Flashcards.kilozdazolik.Data;

public class FlashcardRepository
{
    public void InsertCard(Flashcard flashcard)
    {
        try
        {
            using (var conn = Database.GetConnection())
            {
                var sql = "INSERT INTO dbo.flashcards (stack_id, front, back) VALUES (@StackId, @Front, @Back)";
                conn.Execute(sql, flashcard);
            }
        }
        catch (SqlException e)
        {
            throw new Exception("Database operation failed", e);
        }
    }

    public List<Flashcard> GetCardsByStack(int stackId)
    {
        try
        {
            using (var conn = Database.GetConnection())
            {
                var sql = "SELECT * FROM dbo.flashcards WHERE stack_id = @StackId";
                var flashcards = conn.Query<Flashcard>(sql, new { StackId = stackId });
                return flashcards.ToList();
            }
        }
        catch (SqlException e)
        {
            throw new Exception("Database operation failed", e);
        }
    }
    
    public void UpdateCard (Flashcard flashcard) {}
    
    public void DeleteCard (int cardId) {}
}