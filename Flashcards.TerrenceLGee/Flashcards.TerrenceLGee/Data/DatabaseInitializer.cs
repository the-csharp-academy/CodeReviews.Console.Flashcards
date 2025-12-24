using Dapper;
using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.Data.SqlStatements;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Flashcards.TerrenceLGee.Data;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly ConnectionString _connectionString;
    private readonly ILogger<DatabaseInitializer> _logger;
    private string _errorMessage = string.Empty;

    public DatabaseInitializer(
        ConnectionString connectionString,
        ILogger<DatabaseInitializer> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }


    public async Task InitializeDatabaseAsync()
    {
        try
        {
            var masterConnectionString = new SqlConnectionStringBuilder(_connectionString.Value)
            {
                InitialCatalog = "master"
            }.ConnectionString;
            
            await using (var connection = new SqlConnection(masterConnectionString))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(DatabaseInitializationStatements.CreateDatabaseStatement);
                await connection.ExecuteAsync(DatabaseInitializationStatements.CreateStudyStackTable);
                await connection.ExecuteAsync(DatabaseInitializationStatements.CreateFlashcardTable);
                await connection.ExecuteAsync(DatabaseInitializationStatements.CreateStudySessionTable);
                await connection.ExecuteAsync(DatabaseInitializationStatements.CreateSessionFlashcardTable);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(DatabaseInitializer)}\n" +
                            $"Method: {nameof(InitializeDatabaseAsync)}\n" +
                            $"There was an error initializing the database: {ex.Message}\n";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(DatabaseInitializer)}\n" +
                            $"Method: {nameof(InitializeDatabaseAsync)}\n" +
                            $"There was an unexpected error initializing the database: {ex.Message}\n";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            throw;
        }
    }
}