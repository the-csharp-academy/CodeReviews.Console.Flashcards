using Flashcards.DTOs;
using Flashcards.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Flashcards.Repositories
{
    public class StackRepository
    {
        private readonly string _connectionString;

        public StackRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MSSql")
                ?? throw new InvalidOperationException("Connection string 'MSSql' was not found");
        }

        public async Task CreateStackAsync(string name)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"INSERT INTO Stacks (Name) VALUES (@Name)";

            using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteStackAsync(StackDTO stack)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"DELETE FROM Stacks WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@Id", SqlDbType.Int).Value = stack.Id;

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateStackAsync(StackDTO stack, string newName)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"UPDATE Stacks SET Name = @Name WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@Id", SqlDbType.Int).Value = stack.Id;
            command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = newName;

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Stack>> GetAllStacksAsync()
        {
            var stacks = new List<Stack>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"SELECT Id, Name FROM Stacks";

                using var command = new SqlCommand(query, connection);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var stack = new Stack()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name"))
                    };

                    stacks.Add(stack);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                throw;
            }

            return stacks;
        }

        public async Task<Stack?> GetStackByIdAsync(int stackID)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();

            const string query = @"SELECT Id, Name FROM Stacks WHERE Id = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@Id", SqlDbType.Int).Value = stackID;

            using var reader = await command.ExecuteReaderAsync();

            var stack = new Stack();

            if (!await reader.ReadAsync())
            {
                return null;
            }
            else
            {
                stack.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                stack.Name = reader.GetString(reader.GetOrdinal("Name"));
            }

            return stack;
        }

        public async Task<List<Stack>> GetStacksWithAvailableFlashcardsAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();

            const string query = """SELECT s.Id, s.Name FROM Stacks s WHERE EXISTS (SELECT 1 FROM Flashcards f WHERE f.StackId = s.Id)""";

            using var command = new SqlCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();

            List<Stack> stackList = new();

            while (await reader.ReadAsync())
            {
                var stack = new Stack
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                };

                stackList.Add(stack);
            }

            return stackList;
        }
    }
}
