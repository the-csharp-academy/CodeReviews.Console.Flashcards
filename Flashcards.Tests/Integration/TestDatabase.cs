using System.Data.Common;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CodeReviews.Console.Flashcards.Tests;

public sealed class TestDatabase : IDisposable
{
    public IDatabaseConnectionFactory connectionFactory { get; }
    private readonly DatabaseInitializer _initializer;

    public TestDatabase()
    {
        string databaseName = $"FlashcardsTests_{Guid.NewGuid():N}";

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = @"(localdb)\MSSQLLocalDB",
            InitialCatalog = databaseName,
            IntegratedSecurity = true,
            TrustServerCertificate = true
        };

        connectionFactory = new DatabaseConnectionFactory(builder.ConnectionString);

        _initializer = new DatabaseInitializer(connectionFactory);
    }

    public void Initialize()
    {
        _initializer.Initialize();
    }

    public DbConnection OpenConnection()
    {
        DbConnection connection = connectionFactory.CreateDatabaseConnection();
        connection.Open();
        return connection;
    }

    public void Dispose()
    {
        SqlConnection.ClearAllPools();

        using (DbConnection connection = connectionFactory.CreateMasterConnection())
        {
            connection.Open();

            const string dropDatabaseSql = @"
                IF DB_ID(@DatabaseName) IS NOT NULL
                BEGIN
                    DECLARE @DropDatabaseSql NVARCHAR(MAX);

                    SET @DropDatabaseSql =
                        N'ALTER DATABASE '
                        + QUOTENAME(@DatabaseName)
                        + N' SET SINGLE_USER WITH ROLLBACK IMMEDIATE; '
                        + N'DROP DATABASE '
                        + QUOTENAME(@DatabaseName)
                        + N';';

                    EXEC sys.sp_executesql @DropDatabaseSql;
                END;";

            connection.Execute(dropDatabaseSql, new { DatabaseName = connectionFactory.databaseName });
        }
    }
}