using Dapper;
using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.Data.SqlStatements;
using Flashcards.TerrenceLGee.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Flashcards.TerrenceLGee.Data.Repositories;

public class StudyStackRepository : IStudyStackRepository
{
    private readonly ConnectionString _connectionString;
    private readonly ILogger<StudyStackRepository> _logger;
    private string _errorMessage = string.Empty;

    public StudyStackRepository(
        ConnectionString connectionString,
        ILogger<StudyStackRepository> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }


    public async Task<int> AddStackAsync(StudyStack stack)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    Subject = stack.Subject, 
                    Name = stack.Name 
                };

                return await connection.ExecuteAsync(StudyStackStatements.InsertStack, parameters);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(AddStackAsync)}\n" +
                            $"There was an error adding the study stack to the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(AddStackAsync)}\n" +
                            $"There was an unexpected error adding the study stack to the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
    }

    public async Task<int> UpdateStackAsync(StudyStack stack)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    Id = stack.Id,
                    Subject = stack.Subject,
                    Name = stack.Name
                };

                return await connection.ExecuteAsync(StudyStackStatements.UpdateStack, parameters);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(UpdateStackAsync)}\n" +
                            $"There was an error updating stack {stack.Id} from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(UpdateStackAsync)}\n" +
                            $"There was an unexpected error updating stack {stack.Id} from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
    }

    public async Task<int> DeleteStackAsync(int stackId)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var parameters = new { Id = stackId };

                return await connection.ExecuteAsync(StudyStackStatements.DeleteStack, parameters);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(DeleteStackAsync)}\n" +
                            $"There was an error deleting stack {stackId} from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(DeleteStackAsync)}\n" +
                            $"There was an unexpected error deleting stack {stackId} from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
    }

    public async Task<StudyStack?> GetStackAsync(int stackId)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var stackLookup = new Dictionary<int, StudyStack>();
                var flashcardLookup = new Dictionary<int, Flashcard>();
                var sessionLookup = new Dictionary<int, StudySession>();

                var parameters = new { Id = stackId };

                var stack = await connection
                    .QueryAsync<StudyStack?, Flashcard?, StudySession?, StudyStack?>(StudyStackStatements.GetStack,
                        (st, f, s) =>
                        {
                            if (st is null) return null;
                            if (!stackLookup.TryGetValue(st.Id, out var retrievedStack))
                            {
                                stackLookup.Add(st.Id, retrievedStack = st);
                            }

                            if (f is null) return retrievedStack;
                            if (!flashcardLookup.TryGetValue(f.Id, out var flashcard))
                            {
                                flashcardLookup.Add(f.Id, flashcard = f);
                                retrievedStack.Flashcards.Add(flashcard);
                            }

                            if (s is null) return retrievedStack;
                            if (!sessionLookup.TryGetValue(s.Id, out var session))
                            {
                                sessionLookup.TryAdd(s.Id, session = s);
                                retrievedStack.StudySessions.Add(session);
                            }

                            return retrievedStack;
                        }, parameters);

                return stack.ToList().FirstOrDefault();
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(GetStackAsync)}\n" +
                            $"There was an error retrieving stack {stackId} from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return null;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(GetStackAsync)}\n" +
                            $"There was an unexpected error retrieving stack {stackId} from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return null;
        }
    }

    public async Task<List<StudyStack>> GetStacksAsync()
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var stackLookup = new Dictionary<int, StudyStack>();
                var flashcardLookup = new Dictionary<int, Flashcard>();
                var sessionLookup = new Dictionary<int, StudySession>();

                var stacks = await connection
                    .QueryAsync<StudyStack?, Flashcard?, StudySession?, StudyStack>(StudyStackStatements.GetStacks,
                        (st, f, s) =>
                        {
                            if (st is null) return null!;
                            if (!stackLookup.TryGetValue(st.Id, out var retrievedStack))
                            {
                                stackLookup.TryAdd(st.Id, retrievedStack = st);
                            }

                            if (f is null) return retrievedStack;
                            if (!flashcardLookup.TryGetValue(f.Id, out var flashcard))
                            {
                                flashcardLookup.TryAdd(f.Id, flashcard = f);
                                retrievedStack.Flashcards.Add(flashcard);
                            }

                            if (s is null) return retrievedStack;
                            if (!sessionLookup.TryGetValue(s.Id, out var session))
                            {
                                sessionLookup.TryAdd(s.Id, session = s);
                                retrievedStack.StudySessions.Add(session);
                            }

                            return retrievedStack;
                        });
                return stacks.Distinct().ToList();
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(GetStacksAsync)}\n" +
                            $"There was an error retrieving the study stacks from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return [];
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(GetStacksAsync)}\n" +
                            $"There was an unexpected error retrieving the study stacks from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return [];
        }
    }

    public async Task<int> GetStackCountAsync()
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                return await connection.ExecuteScalarAsync<int>(StudyStackStatements.GetStackCount);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(GetStackCountAsync)}\n" +
                            $"There was an error retrieving the count of study stacks from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(GetStackCountAsync)}\n" +
                            $"There was an unexpected error retrieving the count of study stacks from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return -1;
        }
    }

    public async Task<Dictionary<int, string>> GetStackNameAndIdAsync()
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                return (await connection.QueryAsync<StudyStack>(StudyStackStatements.GetStackNameAndId))
                    .ToDictionary(
                        row => row.Id,
                        row => row.Name);

            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(GetStackNameAndIdAsync)}\n" +
                            $"There was an error retrieving the ids and names " +
                            $"of the study stacks from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return [];
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(GetStackNameAndIdAsync)}\n" +
                            $"There was an unexpected error retrieving the ids and names " +
                            $"of the study stacks from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return [];
        }
    }

    public async Task<string> GetStackNameAsync(int stackId)
    {
        try
        {
            await using (var connection = new SqlConnection(_connectionString.Value))
            {
                await connection.OpenAsync();

                var parameters = new { Id = stackId };

                return await connection.QuerySingleAsync<string>(StudyStackStatements.GetStackName, parameters);
            }
        }
        catch (SqlException ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(GetStackNameAsync)}\n" +
                            $"There was an error retrieving the name " +
                            $"of the study stack {stackId} from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return string.Empty;
        }
        catch (Exception ex)
        {
            _errorMessage = $"\nClass: {nameof(StudyStackRepository)}\n" +
                            $"Method: {nameof(GetStackNameAsync)}\n" +
                            $"There was an unexpected error retrieving the name " +
                            $"of study stack {stackId} from the database: {ex.Message}";
            _logger.LogError(ex, "{msg}\n\n", _errorMessage);
            return string.Empty;
        }
    }
}