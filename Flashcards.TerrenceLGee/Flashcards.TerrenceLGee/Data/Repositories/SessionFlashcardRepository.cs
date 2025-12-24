using Dapper;
using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.Data.SqlStatements;
using Flashcards.TerrenceLGee.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Flashcards.TerrenceLGee.Data.Repositories;

public class SessionFlashcardRepository : ISessionFlashcardRepository
{
    private readonly ConnectionString _connectionString;
    private readonly ILogger<SessionFlashcardRepository> _logger;
    private string _errorMessage = string.Empty;

    public SessionFlashcardRepository(
        ConnectionString connectionString,
        ILogger<SessionFlashcardRepository> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }


    public async Task<int> AddSessionFlashcardAsync(SessionFlashcard sessionFlashcard)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    SessionId = sessionFlashcard.SessionId,
                    FlashcardId = sessionFlashcard.FlashcardId,
                    Question = sessionFlashcard.Question,
                    Answer = sessionFlashcard.Answer,
                    UserAnswer = sessionFlashcard.UserAnswer,
                    IsCorrect = sessionFlashcard.IsCorrect,
                    DisplayPosition = sessionFlashcard.DisplayPosition
                };

                return await connection.ExecuteAsync(SessionFlashcardStatements.InsertSessionFlashcard, parameters);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(SessionFlashcardRepository)}\n" +
                            $"Method: {nameof(AddSessionFlashcardAsync)}\n" +
                            $"There was an error adding the session flashcard for" +
                            $" session {sessionFlashcard.SessionId} and flashcard {sessionFlashcard.FlashcardId}" +
                            $"to the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(SessionFlashcardRepository)}\n" +
                            $"Method: {nameof(AddSessionFlashcardAsync)}\n" +
                            $"There was an unexpected error adding the session flashcard for" +
                            $" session {sessionFlashcard.SessionId} and flashcard {sessionFlashcard.FlashcardId}" +
                            $"to the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
    }
}