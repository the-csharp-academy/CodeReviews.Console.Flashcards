using Dapper;
using Microsoft.Data.SqlClient;

namespace CodeReviews.Console.Flashcards;

public sealed class DatabaseInitializer
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly string _schemaScriptPath;
    public DatabaseInitializer(IDatabaseConnectionFactory connectionFactory, string schemaScriptPath)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

        if (string.IsNullOrWhiteSpace(schemaScriptPath))
        {
            throw new ArgumentException(
                "The schema script path cannot be empty.",
                nameof(schemaScriptPath));
        }

        _schemaScriptPath = schemaScriptPath;
    }

    public void Initialize()
    {
        if (!File.Exists(_schemaScriptPath))
            throw new FileNotFoundException("The database schema script file was not found.", _schemaScriptPath);

        EnsureDatabaseExists();
        ExecuteSchemaScript();
    }

    private void EnsureDatabaseExists()
    {
        using var connection = _connectionFactory.CreateConnection();
        if (string.IsNullOrWhiteSpace(connection.ConnectionString))
            throw new InvalidOperationException("Database connection string must not be empty.");

        var builder = new SqlConnectionStringBuilder(connection.ConnectionString);

        var databaseName = builder.InitialCatalog;

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException("The connection string must specify a database name.");

        var masterBuilder = new SqlConnectionStringBuilder(connection.ConnectionString)
        {
            InitialCatalog = "master"
        };

        using var masterConnection = new SqlConnection(masterBuilder.ConnectionString);
        masterConnection.Open();

        const string createDatabaseSql =
            @"IF DB_ID(@DatabaseName) IS NULL
                BEGIN
                    DECLARE @CreateDatabaseSql NVARCHAR(MAX);

                    SET @CreateDatabaseSql =
                        N'CREATE DATABASE ' + QUOTENAME(@DatabaseName);

                    EXEC sys.sp_executesql @CreateDatabaseSql;
                END;";

        masterConnection.Execute(createDatabaseSql, new { DatabaseName = databaseName });
    }

    private void ExecuteSchemaScript()
    {
        var script = File.ReadAllText(_schemaScriptPath);
        if (string.IsNullOrWhiteSpace(script))
            throw new InvalidOperationException($"The database schema script '{_schemaScriptPath}' is empty.");

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        connection.Execute(script);
    }
}
