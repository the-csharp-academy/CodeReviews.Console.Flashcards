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

        public List<int> GetStatByMonth(int year)
        {
            var list = new List<(int Month, int Count)>();
            var res = new List<int>(new int[12]);
            string query = "SELECT MONTH(Date), COUNT(*) FROM Sessions WHERE YEAR(Date) = @Year GROUP BY MONTH(Date) ORDER BY MONTH(Date) ASC;";

            using (var conn = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(query,conn);
                command.Parameters.AddWithValue("@Year", year);
                conn.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add((reader.GetInt32(0), reader.GetInt32(1)));
                    }
                }
            }
            foreach(var pair in list)
            {
                res[pair.Month-1] = pair.Count;
            }
            return res;
        }
    }
}
