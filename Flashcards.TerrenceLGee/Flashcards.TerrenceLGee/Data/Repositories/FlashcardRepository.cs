using Dapper;
using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.Data.SqlStatements;
using Flashcards.TerrenceLGee.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Flashcards.TerrenceLGee.Data.Repositories;

public class FlashcardRepository : IFlashcardRepository
{
    private readonly ConnectionString _connectionString;
    private readonly ILogger<FlashcardRepository> _logger;
    private string _errorMessage = string.Empty;

    public FlashcardRepository(
        ConnectionString connectionString,
        ILogger<FlashcardRepository> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }


    public async Task<int> AddFlashcardAsync(Flashcard flashcard)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    StackId = flashcard.StackId,
                    Question = flashcard.Question,
                    Answer = flashcard.Answer,
                    Position = flashcard.Position
                };

                return await connection.ExecuteAsync(FlashcardStatements.InsertFlashcard, parameters);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(FlashcardRepository)}\n" +
                            $"Method: {nameof(AddFlashcardAsync)}\n" +
                            $"There was an error adding flashcard to stack " +
                            $"{flashcard.StackId} in the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(FlashcardRepository)}\n" +
                            $"Method: {nameof(AddFlashcardAsync)}\n" +
                            $"There was an unexpected error adding flashcard to stack" +
                            $" {flashcard.StackId} in the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
    }

    public async Task<int> UpdateFlashcardAsync(Flashcard flashcard)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    Id = flashcard.Id,
                    StackId = flashcard.StackId,
                    Question = flashcard.Question,
                    Answer = flashcard.Answer,
                    Position = flashcard.Position
                };

                return await connection.ExecuteAsync(FlashcardStatements.UpdateFlashcard, parameters);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(FlashcardRepository)}\n" +
                            $"Method: {nameof(UpdateFlashcardAsync)}\n" +
                            $"There was an error updating flashcard {flashcard.Id} in stack" +
                            $" {flashcard.StackId} in the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(FlashcardRepository)}\n" +
                            $"Method: {nameof(UpdateFlashcardAsync)}\n" +
                            $"There was an unexpected error updating flashcard {flashcard.Id} in stack" +
                            $" {flashcard.StackId} in the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
    }

    public async Task<int> DeleteFlashcardAsync(int stackId, int position)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    Position = position,
                    StackId = stackId
                };

                return await connection.ExecuteAsync(FlashcardStatements.DeleteFlashcard, parameters);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(FlashcardRepository)}\n" +
                            $"Method: {nameof(DeleteFlashcardAsync)}\n" +
                            $"There was an error deleting flashcard at position {position} from stack {stackId}" +
                            $" from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(FlashcardRepository)}\n" +
                            $"Method: {nameof(DeleteFlashcardAsync)}\n" +
                            $"There was an unexpected error deleting flashcard at position {position} from stack {stackId}" +
                            $" from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
    }

    public async Task<Flashcard?> GetFlashcardAsync(int stackId, int position)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    Position = position,
                    StackId = stackId
                };

                return await connection
                    .QueryFirstOrDefaultAsync<Flashcard>(FlashcardStatements.GetFlashcard, parameters);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(FlashcardRepository)}\n" +
                            $"Method: {nameof(GetFlashcardAsync)}\n" +
                            $"There was an error retrieving flashcard at position {position} in stack " +
                            $"{stackId} from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return null;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(FlashcardRepository)}\n" +
                            $"Method: {nameof(GetFlashcardAsync)}\n" +
                            $"There was an unexpected error retrieving flashcard at position {position} in stack " +
                            $"{stackId} from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return null;
        }
    }

    public async Task<List<Flashcard>> GetFlashcardsAsync(int stackId)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var parameters = new { StackId = stackId };

                return (await connection.QueryAsync<Flashcard>(FlashcardStatements.GetFlashcards, parameters)).ToList();
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(FlashcardRepository)}\n" +
                            $"Method: {nameof(GetFlashcardAsync)}\n" +
                            $"There was an error retrieving the flashcards in stack: {stackId}" +
                            $" from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return [];
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(FlashcardRepository)}\n" +
                            $"Method: {nameof(GetFlashcardsAsync)}\n" +
                            $"There was an unexpected error retrieving the flashcards in stack: {stackId} " +
                            $"from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return [];
        }
    }

    public async Task<int> GetFlashcardCountAsync(int stackId)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var parameters = new { StackId = stackId };

                return await connection.ExecuteScalarAsync<int>(FlashcardStatements.GetFlashcardCount, parameters);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(FlashcardRepository)}\n" +
                            $"Method: {nameof(GetFlashcardCountAsync)}\n" +
                            $"There was an error retrieving the count of flashcards for stack {stackId} " +
                            $"from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(FlashcardRepository)}\n" +
                            $"Method: {nameof(GetFlashcardCountAsync)}\n" +
                            $"There was an unexpected error retrieving the count of flashcards for stack {stackId} " +
                            $"from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
    }
}