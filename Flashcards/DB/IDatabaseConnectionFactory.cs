using System.Data.Common;
using Microsoft.Data.SqlClient;
namespace CodeReviews.Console.Flashcards;

public interface IDatabaseConnectionFactory
{
    DbConnection CreateConnection();
}

public sealed class DatabaseConnectionFactory : IDatabaseConnectionFactory
{
    private readonly string _connectionString;

    public DatabaseConnectionFactory(string connString)
    {
        if (string.IsNullOrWhiteSpace(connString))
            throw new ArgumentException("The database connection string cannot be empty.", nameof(connString));
        _connectionString = connString;
    }

    public DbConnection CreateConnection() => new SqlConnection(_connectionString);
}