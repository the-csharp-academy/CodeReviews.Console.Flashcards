using FlashcardsApp.Models;
using FlashcardsApp.DTOs;
using Microsoft.Data.SqlClient;

namespace FlashcardsApp.Repositories
{
    public class FlashcardRepository
    {
        private readonly string _connectionString;

        public FlashcardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Flashcard> GetAll()
        {
            List<Flashcard> ls = new List<Flashcard>();
            string query = "SELECT Front, Back FROM Flashcards;";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, conn);
                conn.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ls.Add(new Flashcard
                        {
                            Id = reader.GetInt32(0),
                            StackId = reader.GetInt32(1),
                            Front = reader.GetString(2),
                            Back = reader.GetString(3),
                        });
                    }
                }

            }
            return ls;
        }

        public List<Flashcard> GetAllOfStack(FStack fstack)
        {
            List<Flashcard> ls = new List<Flashcard>();
            string query = "SELECT Id, StackId, Front, Back FROM Flashcards WHERE StackId = @StackId;";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@StackId", fstack.Id);
                conn.Open();

                using (var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        ls.Add(new Flashcard
                        {
                            Id = reader.GetInt32(0),
                            StackId = reader.GetInt32(1),
                            Front = reader.GetString(2),
                            Back = reader.GetString(3),
                        });
                    }
            }
            return ls;
            }
        }

        public bool Insert(FlashcardDTO flashcard, FStack fs)
        {
            string query = "INSERT INTO Flashcards (StackId, Front, Back) VALUES(@StackId, @Front, @Back);";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@StackId", fs.Id);
                command.Parameters.AddWithValue("@Front", flashcard.Front);
                command.Parameters.AddWithValue("@Back", flashcard.Back);
                conn.Open();

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Update(Flashcard flashcard)
        {
            string query = "UPDATE Flashcards SET Front = @Front, Back = @Back WHERE Id = @Id;";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Id", flashcard.Id);
                command.Parameters.AddWithValue("@Front", flashcard.Front);
                command.Parameters.AddWithValue("@Back", flashcard.Back);
                conn.Open();

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(Flashcard flashcard)
        {
            string query = "DELETE FROM Flashcards WHERE Id = @Id;";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Id", flashcard.Id);
                conn.Open();

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
