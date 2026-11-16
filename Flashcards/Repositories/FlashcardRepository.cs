using Flashcards.DTOs;
using Flashcards.Entities;
using Flashcards.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Flashcards.Repositories
{
    public class FlashcardRepository : IFlashcardRepository
    {
        private readonly string _connectionString;

        public FlashcardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MSSql")
                ?? throw new InvalidOperationException("Connection string 'MSSql' was not found");
        }

        public async Task<List<Flashcard>> GetAllFlashcardsAsync(int? stackId)
        {
            var flashcards = new List<Flashcard>();

            await using var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();

            string query = "";

            if (stackId != null)
            {
                query = """SELECT Id, Front, Back, StackID FROM Flashcards WHERE StackId = @StackId ORDER BY Id""";
            }
            else
            {
                query = """SELECT Id, Front, Back, StackID FROM Flashcards ORDER BY Id""";
            }

            await using var command = new SqlCommand(query, connection);

            if (stackId != null)
            {
                command.Parameters.Add("@StackId", SqlDbType.Int).Value = stackId;
            }

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var flashcard = new Flashcard
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Front = reader.GetString(reader.GetOrdinal("Front")),
                    Back = reader.GetString(reader.GetOrdinal("Back")),
                    StackID = reader.GetInt32(reader.GetOrdinal("StackID"))
                };

                flashcards.Add(flashcard);
            }

            return flashcards;
        }

        public async Task CreateFlashcardAsync(FlashcardDTO flashcard)
        {
            await using var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();

            const string query = @"INSERT INTO Flashcards (Front, Back, StackID) Values (@Front, @Back, @StackID)";

            await using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@Front", SqlDbType.NVarChar).Value = flashcard.Front;
            command.Parameters.Add("@Back", SqlDbType.NVarChar).Value = flashcard.Back;
            command.Parameters.Add("@StackId", SqlDbType.Int).Value = flashcard.StackId;

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateFlashcardAsync(FlashcardDTO flashcard)
        {
            await using var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();

            const string query = @"UPDATE Flashcards SET StackId = @StackId, Front = @Front, Back = @Back WHERE Id = @Id";

            await using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@StackId", SqlDbType.Int).Value = flashcard.StackId;
            command.Parameters.Add("@Front", SqlDbType.NVarChar).Value = flashcard.Front;
            command.Parameters.Add("@Back", SqlDbType.NVarChar).Value = flashcard.Back;
            command.Parameters.Add("@Id", SqlDbType.Int).Value = flashcard.Id;

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteFlashcardAsync(FlashcardDTO flashcard)
        {
            await using var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();

            const string query = @"DELETE FROM Flashcards WHERE Id = @Id";

            await using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@Id", SqlDbType.Int).Value = flashcard.Id;

            await command.ExecuteNonQueryAsync();
        }
    }
}
