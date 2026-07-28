using FlashcardsApp.DTOs;
using FlashcardsApp.Models;
using Microsoft.Data.SqlClient;

namespace FlashcardsApp.Repositories
{
    public class SessionRepository
    {
        private readonly string _connectionString;

        public SessionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Insert(SessionDTO ss, FStack fs)
        {
            string query = "INSERT INTO Sessions (StackId, Date, Score) VALUES(@StackId, @Date, @Score);";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@StackId", fs.Id);
                command.Parameters.AddWithValue("@Date", ss.Date);
                command.Parameters.AddWithValue("@Score", ss.Score);
                conn.Open();

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public Dictionary<string, List<int>> GetStatByMonth(int year)
        {
            var result = new Dictionary<string, List<int>>();
            string query = @"
                SELECT s.Name, MONTH(ss.Date), COUNT(*)
                FROM Sessions ss
                INNER JOIN Stacks s ON ss.StackId = s.Id
                WHERE YEAR(ss.Date) = @Year
                GROUP BY s.Name, MONTH(ss.Date)
                ORDER BY s.Name, MONTH(ss.Date) ASC;";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Year", year);
                conn.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string stackName = reader.GetString(0);
                        int month = reader.GetInt32(1);
                        int count = reader.GetInt32(2);

                        if (!result.ContainsKey(stackName))
                        {
                            result[stackName] = new List<int>(new int[12]);
                        }
                        result[stackName][month - 1] = count;
                    }
                }
            }
            return result;
        }
    }
}
