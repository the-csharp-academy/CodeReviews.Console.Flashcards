using System.Data.Common;
using Microsoft.Data.SqlClient;
namespace CodeReviews.Console.Flashcards;

public interface IDatabaseConnectionFactory
{
    string databaseName { get; }
    DbConnection CreateDatabaseConnection();
    DbConnection CreateMasterConnection();
}

public sealed class DatabaseConnectionFactory : IDatabaseConnectionFactory
{
    private readonly string _databaseConnectionString;
    private readonly string _masterConnectionString;
    public string databaseName { get; }

    public DatabaseConnectionFactory(string connString)
    {
        if (string.IsNullOrWhiteSpace(connString))
            throw new ArgumentException("The database connection string cannot be empty.", nameof(connString));

        var databaseBuilder = new SqlConnectionStringBuilder(connString);

        if (string.IsNullOrWhiteSpace(databaseBuilder.InitialCatalog))
            throw new ArgumentException("The connection string must specify a database.", nameof(connString));

        databaseName = databaseBuilder.InitialCatalog;
        _databaseConnectionString = databaseBuilder.ConnectionString;

        var masterBuilder = new SqlConnectionStringBuilder(databaseBuilder.ConnectionString)
        {
            InitialCatalog = "master"
        };

        _masterConnectionString = masterBuilder.ConnectionString;
    }

    public DbConnection CreateDatabaseConnection() => new SqlConnection(_databaseConnectionString);
    public DbConnection CreateMasterConnection() => new SqlConnection(_masterConnectionString);
}