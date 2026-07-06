using Microsoft.Data.SqlClient;
using FlashcardsApp.Models;
using FlashcardsApp.DTOs;

namespace FlashcardsApp.Repositories
{
    public class StackRepository
    {
        private readonly string _connectionString;

        public StackRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<FStack> GetAll()
        {
            var stacks = new List<FStack>();
            string query = "SELECT Id, Name FROM Stacks;";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, conn);
                conn.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stacks.Add(new FStack
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return stacks;
        }

        public bool Insert(FStackDTO fstack)
        {
            string query = "INSERT INTO Stacks (Name) VALUES(@Name);";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Name", fstack.Name);
                conn.Open();

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Update(FStack fstack)
        {
            string query = "UPDATE Stacks SET Name = @Name WHERE Id = @Id;";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Id", fstack.Id);
                command.Parameters.AddWithValue("@Name", fstack.Name);
                conn.Open();

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(FStack fstack)
        {
            string query = "DELETE FROM Stacks WHERE Id = @Id;";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Id", fstack.Id);
                conn.Open();

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
