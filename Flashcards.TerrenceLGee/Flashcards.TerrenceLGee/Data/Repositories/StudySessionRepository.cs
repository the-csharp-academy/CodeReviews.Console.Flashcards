using Dapper;
using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.Data.SqlStatements;
using Flashcards.TerrenceLGee.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Flashcards.TerrenceLGee.Data.Repositories;

public class StudySessionRepository : IStudySessionRepository
{
    private readonly ConnectionString _connectionString;
    private readonly ILogger<StudySessionRepository> _logger;
    private string _errorMessage = string.Empty;

    public StudySessionRepository(
        ConnectionString connectionString,
        ILogger<StudySessionRepository> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }


    public async Task<int> AddStudySessionAsync(StudySession studySession)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    StackId = studySession.StackId,
                    TotalQuestions = studySession.TotalQuestions,
                    Correct = studySession.Correct,
                    Incorrect = studySession.Incorrect,
                    Score = studySession.Score,
                    SessionDuration = studySession.SessionDuration
                };



                return await connection.QuerySingleAsync<int>(StudySessionStatements.InsertStudySession, parameters);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(StudySessionRepository)}\n" +
                            $"Method: {nameof(AddStudySessionAsync)}\n" +
                            $"There was an error adding the study session associated with stack" +
                            $" {studySession.StackId} to the database: {ex.Message}\n";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(StudySessionRepository)}\n" +
                            $"Method: {nameof(AddStudySessionAsync)}\n" +
                            $"There was an unexpected error adding the study session associated with stack" +
                            $" {studySession.StackId} to the database: {ex.Message}\n";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
    }

    public async Task<StudySession?> GetStudySessionAsync(int stackId, int sessionId)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var sessionLookup = new Dictionary<int, StudySession>();
                var sessionFlashcardLookup = new Dictionary<int, SessionFlashcard>();
                
                var parameters = new
                {
                    Id = sessionId,
                    StackId = stackId
                };

                var sessions = await connection.QueryAsync<StudySession?, SessionFlashcard?, StudySession?>(
                    StudySessionStatements.GetStudySession, (s, sf) =>
                    {
                        if (s is null) return null;
                        if (!sessionLookup.TryGetValue(s.Id, out var retrievedSession))
                        {
                            sessionLookup.TryAdd(s.Id, retrievedSession = s);
                        }

                        if (sf is null) return retrievedSession;
                        if (!sessionFlashcardLookup.TryGetValue(sf.FlashcardId, out var sessionFlashcard))
                        {
                            sessionFlashcardLookup.TryAdd(sf.FlashcardId, sessionFlashcard = sf);
                            retrievedSession.SessionFlashcards.Add(sessionFlashcard);
                        }

                        return retrievedSession;
                    }, parameters, splitOn: "SessionId");

                return sessions.FirstOrDefault();
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(StudySessionRepository)}\n" +
                            $"Method: {nameof(GetStudySessionAsync)}\n" +
                            $"There was an error retrieving study session {sessionId} associated with stack " +
                            $"{stackId} from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return null;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(StudySessionRepository)}\n" +
                            $"Method: {nameof(GetStudySessionAsync)}\n" +
                            $"There was an unexpected error retrieving study session {sessionId} associated with stack " +
                            $"{stackId} from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return null;
        }
    }

    public async Task<List<StudySession>> GetStudySessionsAsync(int stackId)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var sessionLookup = new Dictionary<int, StudySession>();
                var sessionFlashcardLookup = new Dictionary<int, SessionFlashcard>();

                var parameters = new { StackId = stackId };

                var sessions = await connection
                    .QueryAsync<StudySession?, SessionFlashcard?, StudySession>(StudySessionStatements.GetStudySessions,
                        (s, sf) =>
                        {
                            if (s is null) return null!;
                            if (!sessionLookup.TryGetValue(s.Id, out var retrievedSession))
                            {
                                sessionLookup.Add(s.Id, retrievedSession = s);
                            }

                            if (sf is null) return retrievedSession;
                            if (!sessionFlashcardLookup.TryGetValue(sf.FlashcardId, out var sessionFlashcard))
                            {
                                sessionFlashcardLookup.Add(sf.FlashcardId, sessionFlashcard = sf);
                                retrievedSession.SessionFlashcards.Add(sessionFlashcard);
                            }

                            return retrievedSession;
                        }, parameters, splitOn: "SessionId");

                return sessions.Distinct().ToList();
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(StudySessionRepository)}\n" +
                            $"Method: {nameof(GetStudySessionsAsync)}\n" +
                            $"There was an error retrieving the study sessions associated with stack {stackId}" +
                            $" from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return [];
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(StudySessionRepository)}\n" +
                            $"Method: {nameof(GetStudySessionsAsync)}\n" +
                            $"There was an unexpected error retrieving the study sessions associated with stack {stackId}" +
                            $" from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return [];
        }
    }

    public async Task<int> GetStudySessionCountAsync(int stackId)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var sessionLookup = new Dictionary<int, StudySession>();
                var sessionFlashcardLookup = new Dictionary<int, SessionFlashcard>();
                var parameters = new { StackId = stackId };

                return await connection.ExecuteScalarAsync<int>(StudySessionStatements.GetStudySessionCount,
                    parameters);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(StudySessionRepository)}\n" +
                            $"Method: {nameof(GetStudySessionCountAsync)}\n" +
                            $"There was an error retrieving the study session count of the " +
                            $"sessions associated with stack {stackId}: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(StudySessionRepository)}\n" +
                            $"Method: {nameof(GetStudySessionCountAsync)}\n" +
                            $"There was an unexpected error retrieving the study session count of the " +
                            $"sessions associated with stack {stackId}: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
    }
}