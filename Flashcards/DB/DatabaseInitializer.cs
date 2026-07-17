using Dapper;

namespace CodeReviews.Console.Flashcards;

public sealed class DatabaseInitializer
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly string _schemaScriptPath;
    public DatabaseInitializer(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _schemaScriptPath = Path.Combine(AppContext.BaseDirectory, "Scripts", "Schema.sql");
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
        using (var connection = _connectionFactory.CreateMasterConnection())
        {
            connection.Open();

            const string createDatabaseSql =
            @"IF DB_ID(@DatabaseName) IS NULL
                BEGIN
                    DECLARE @CreateDatabaseSql NVARCHAR(MAX);

                    SET @CreateDatabaseSql =
                        N'CREATE DATABASE ' + QUOTENAME(@DatabaseName);

                    EXEC sys.sp_executesql @CreateDatabaseSql;
                END;";
            connection.Execute(createDatabaseSql, new { DatabaseName = _connectionFactory.databaseName });
        }
    }

    private void ExecuteSchemaScript()
    {
        var script = File.ReadAllText(_schemaScriptPath);
        using (var connection = _connectionFactory.CreateDatabaseConnection())
        {
            connection.Open();
            connection.Execute(script);
        }
    }
}
