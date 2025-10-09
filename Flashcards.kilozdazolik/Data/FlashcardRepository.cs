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
                var sql = @"SELECT card_id AS CardId, 
                               stack_id AS StackId, 
                               front AS Front, 
                               back AS Back 
                        FROM dbo.flashcards 
                        WHERE stack_id = @StackId";
                var flashcards = conn.Query<Flashcard>(sql, new { StackId = stackId });
                return flashcards.ToList();
            }
        }
        catch (SqlException e)
        {
            throw new Exception("Database operation failed", e);
        }
    }

    public void UpdateCard(Flashcard flashcard)
    {
        try
        {
            using (var conn = Database.GetConnection())
            {
                var sql = "UPDATE dbo.flashcards SET front = @Front, back = @Back, stack_id = @StackId WHERE card_id = @CardId";
                conn.Execute(sql, new 
                {
                    Front = flashcard.Front,
                    Back = flashcard.Back,
                    StackId = flashcard.StackId,
                    CardId = flashcard.CardId 
                });
            }
        }
        catch (SqlException ex) when (ex.Number == 2627) // duplicate key
        {
            throw new InvalidOperationException("A flashcard with this name already exists.", ex);
        }
        catch (SqlException e)
        {
            throw new Exception("Database operation failed", e);
        }
    }

    public void DeleteCard(Flashcard flashcard)
    {
        try
        {
            using (var conn = Database.GetConnection())
            {
                var sql = "DELETE FROM dbo.flashcards WHERE card_id=@CardId";
                conn.Execute(sql, flashcard);
            }
        }
        catch (SqlException e)
        {
            throw new Exception("Database operation failed", e);
        }
    }
}