using Flashcards.DTOs;
using Flashcards.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Flashcards.Repositories
{
    internal class StudySessionRepository
    {
        private readonly string _connectionString;
        public StudySessionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MSSql")
                ?? throw new InvalidOperationException("Connection string 'MSSql' was not found");
        }

        public async Task SaveStudySessionAsync(StudySessionDTO studySession)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"INSERT INTO StudySessions (StackID, StartTime, Score, EndTime, Duration, Percentage, TotalCards)"
                        + "VALUES (@StackID, @StartTime, @Score, @EndTIme, @Duration, @Percentage, @TotalCards)";

            await using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@StackID", SqlDbType.Int).Value = studySession.StackId;
            command.Parameters.Add("@StartTime", SqlDbType.DateTime).Value = studySession.StartTime;
            command.Parameters.Add("@Score", SqlDbType.Int).Value = studySession.Score;
            command.Parameters.Add("@EndTime", SqlDbType.DateTime).Value = studySession.EndTime;
            command.Parameters.Add("@Duration", SqlDbType.Int).Value = studySession.Duration;
            command.Parameters.Add("@Percentage", SqlDbType.Decimal).Value = studySession.Percentage;
            command.Parameters.Add("@TotalCards", SqlDbType.Int).Value = studySession.TotalCards;

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<StudySession>> GetAllStudySessionsAsync()
        {
            var studySessionsList = new List<StudySession>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM StudySessions";

            await using var command = new SqlCommand(query, connection);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var studySession = new StudySession
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    StackId = reader.GetInt32(reader.GetOrdinal("StackId")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                    Score = reader.GetInt32(reader.GetOrdinal("Score")),
                    EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                    Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                    Percentage = Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("Percentage"))),
                    TotalCards = reader.GetInt32(reader.GetOrdinal("TotalCards"))
                };

                studySessionsList.Add(studySession);
            }

            return studySessionsList;
        }

        public async Task<List<StudySession>> GetAllStudySessionsByStackAsync(StackDTO stack)
        {
            var studySessionByStackList = new List<StudySession>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM StudySessions WHERE StackId = @StackId";

            await using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@StackId", SqlDbType.Int).Value = stack.Id;

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var studySession = new StudySession
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    StackId = reader.GetInt32(reader.GetOrdinal("StackId")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                    Score = reader.GetInt32(reader.GetOrdinal("Score")),
                    EndTime = reader.GetDateTime(reader.GetOrdinal("EndTime")),
                    Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                    Percentage = Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("Percentage"))),
                    TotalCards = reader.GetInt32(reader.GetOrdinal("TotalCards"))
                };

                studySessionByStackList.Add(studySession);
            }

            return studySessionByStackList;

        }
    }
}
