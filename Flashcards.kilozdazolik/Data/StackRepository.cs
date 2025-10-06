using Dapper;
using Flashcards.kilozdazolik.Models;
using Microsoft.Data.SqlClient;

namespace Flashcards.kilozdazolik.Data;

public class StackRepository
{
    public void InsertStack(Stack stack)
    {
        try
        {
            using (var conn = Database.GetConnection())
            {
                var sql = "INSERT INTO dbo.stacks (name) VALUES (@Name)";
                conn.Execute(sql, stack);
            }
        }
        catch (SqlException ex) when (ex.Number == 2627) // duplicate key
        {
            throw new InvalidOperationException("A stack with this name already exists.", ex);
        }
        catch (SqlException e)
        {
            throw new Exception("Database operation failed", e);
        }
    }

    public void UpdateStack(Stack stack)
    {
        try
        {
            using (var conn = Database.GetConnection())
            {
                var sql = "UPDATE dbo.stacks SET name=@Name WHERE stack_id=@StackId";
                conn.Execute(sql, stack);
            }
        }
        catch (SqlException ex) when (ex.Number == 2627) // duplicate key
        {
            throw new InvalidOperationException("A stack with this name already exists.", ex);
        }
        catch (SqlException e)
        {
            throw new Exception("Database operation failed", e);
        }
    }

    public void DeleteStack(Stack stack)
    {
        try
        {
            using (var conn = Database.GetConnection())
            {
                var sql = "DELETE FROM dbo.stacks WHERE stack_id=@StackId";
                conn.Execute(sql, stack);
            }
        }
        catch (SqlException e)
        {
            throw new Exception("Database operation failed", e);
        }

    }

    public List<Stack> GetAllStacks()
    {
        try
        {
            using (var conn = Database.GetConnection())
            {
                var sql = "select stack_id as StackId, name as Name from dbo.stacks order by stack_id";
                var stacks = conn.Query<Stack>(sql).ToList();
                return stacks;
            }
        }
        catch (SqlException e)
        {
            throw new Exception("Database operation failed", e);
        }

    }
}