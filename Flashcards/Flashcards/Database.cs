using Microsoft.Data.SqlClient;

namespace Flashcards
{
    public class Database
    {
        public static string connectionString = @"Server = KOMPUTER-K;Database = FlashcardsDb;TrustServerCertificate=True;Integrated Security=True";

        public static SqlConnection GetConnection()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();

            return connection;
        }
    }
}