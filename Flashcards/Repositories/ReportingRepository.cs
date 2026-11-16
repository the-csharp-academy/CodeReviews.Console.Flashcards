using Flashcards.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace Flashcards.Repositories
{
    internal class ReportingRepository
    {
        private readonly string _connectionString;
        public ReportingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MSSql")
                ?? throw new InvalidOperationException("Connection string 'MSSql' was not found");
        }

        public async Task<List<MonthlySessionReportDTO>> CreateMonthlySessionReportDTOList(SqlDataReader reader)
        {
            int id = 1;
            List<MonthlySessionReportDTO> sessionsList = new();

            while (await reader.ReadAsync())
            {
                var sessionDTO = new MonthlySessionReportDTO
                {
                    StackId = id++,
                    StackName = reader.GetString(reader.GetOrdinal("StackName")),
                    Year = reader.GetInt32(reader.GetOrdinal("Year")),
                    January = reader.GetInt32(reader.GetOrdinal("January")),
                    February = reader.GetInt32(reader.GetOrdinal("February")),
                    March = reader.GetInt32(reader.GetOrdinal("March")),
                    April = reader.GetInt32(reader.GetOrdinal("April")),
                    May = reader.GetInt32(reader.GetOrdinal("May")),
                    June = reader.GetInt32(reader.GetOrdinal("June")),
                    July = reader.GetInt32(reader.GetOrdinal("July")),
                    August = reader.GetInt32(reader.GetOrdinal("August")),
                    September = reader.GetInt32(reader.GetOrdinal("September")),
                    October = reader.GetInt32(reader.GetOrdinal("October")),
                    November = reader.GetInt32(reader.GetOrdinal("November")),
                    December = reader.GetInt32(reader.GetOrdinal("December"))
                };

                sessionsList.Add(sessionDTO);
            }

            return sessionsList;
        }
        public async Task<List<MonthlyAvgScoreReportDTO>> CreateMonthlyAvgScoreReportDTOList(SqlDataReader reader)
        {
            int id = 1;
            List<MonthlyAvgScoreReportDTO> sessionsList = new();

            while (await reader.ReadAsync())
            {
                var sessionDTO = new MonthlyAvgScoreReportDTO
                {
                    StackId = id++,
                    StackName = reader.GetString(reader.GetOrdinal("StackName")),
                    Year = reader.GetInt32(reader.GetOrdinal("Year")),
                    January = reader.GetDecimal(reader.GetOrdinal("January")),
                    February = reader.GetDecimal(reader.GetOrdinal("February")),
                    March = reader.GetDecimal(reader.GetOrdinal("March")),
                    April = reader.GetDecimal(reader.GetOrdinal("April")),
                    May = reader.GetDecimal(reader.GetOrdinal("May")),
                    June = reader.GetDecimal(reader.GetOrdinal("June")),
                    July = reader.GetDecimal(reader.GetOrdinal("July")),
                    August = reader.GetDecimal(reader.GetOrdinal("August")),
                    September = reader.GetDecimal(reader.GetOrdinal("September")),
                    October = reader.GetDecimal(reader.GetOrdinal("October")),
                    November = reader.GetDecimal(reader.GetOrdinal("November")),
                    December = reader.GetDecimal(reader.GetOrdinal("December"))
                };

                sessionsList.Add(sessionDTO);
            }

            return sessionsList;
        }

        public async Task<List<MonthlySessionReportDTO>> ShowMonthlyStudySessionsByYearAsync(int year)
        {
            await using var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();

            var query = @"SELECT StackId, StackName, [Year],"
                        + "ISNULL([1], 0) as January,"
                        + "ISNULL([2], 0) as February,"
                        + "ISNULL([3], 0) as March,"
                        + "ISNULL([4], 0) as April,"
                        + "ISNULL([5], 0) as May,"
                        + "ISNULL([6], 0) as June,"
                        + "ISNULL([7], 0) as July,"
                        + "ISNULL([8], 0) as August,"
                        + "ISNULL([9], 0) as September,"
                        + "ISNULL([10], 0) as October,"
                        + "ISNULL([11], 0) as November,"
                        + "ISNULL([12], 0) as December "
                        + "FROM ( SELECT s.Name as StackName, s.Id as StackId, YEAR(ss.StartTime) as [Year], " +
                        "MONTH(ss.StartTime) as [Month], COUNT(ss.Id) as TotalSessions FROM Stacks s " +
                        "INNER JOIN StudySessions ss on ss.StackId = s.Id " +
                        "WHERE ss.StartTime >= DATEFROMPARTS(@Year, 1, 1) AND ss.StartTime < DATEFROMPARTS(@Year + 1, 1, 1) " +
                        "GROUP BY S.Id, S.Name, YEAR(ss.StartTime), MONTH(ss.StartTime)) AS a " +
                        "PIVOT(" +
                        "SUM(TotalSessions) " +
                        "FOR [Month] IN ([1], [2], [3] , [4], [5], [6], [7], [8], [9], [10], [11], [12])) AS b ORDER BY StackName";

            var command = new SqlCommand(query, connection);
            command.Parameters.Add("@Year", SqlDbType.Int).Value = year;

            using var reader = await command.ExecuteReaderAsync();

            return await CreateMonthlySessionReportDTOList(reader);
        }

        public async Task<List<MonthlyAvgScoreReportDTO>> ShowMonthlySessionsAvgByYearAsync(int year)
        {
            await using var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();

            var query = @"SELECT StackId, StackName, [Year],"
                        + "ISNULL([1], 0) as January,"
                        + "ISNULL([2], 0) as February,"
                        + "ISNULL([3], 0) as March,"
                        + "ISNULL([4], 0) as April,"
                        + "ISNULL([5], 0) as May,"
                        + "ISNULL([6], 0) as June,"
                        + "ISNULL([7], 0) as July,"
                        + "ISNULL([8], 0) as August,"
                        + "ISNULL([9], 0) as September,"
                        + "ISNULL([10], 0) as October,"
                        + "ISNULL([11], 0) as November,"
                        + "ISNULL([12], 0) as December "
                        + "FROM ( SELECT s.Name as StackName, s.Id as StackId, YEAR(ss.StartTime) as [Year], " +
                        "MONTH(ss.StartTime) as [Month], AVG(CAST(ss.Percentage AS DECIMAL(5,2))) as AvgSession FROM Stacks s " +
                        "INNER JOIN StudySessions ss on ss.StackId = s.Id " +
                        "WHERE ss.StartTime >= DATEFROMPARTS(@Year, 1, 1) AND ss.StartTime < DATEFROMPARTS(@Year + 1, 1, 1) " +
                        "GROUP BY S.Id, S.Name, YEAR(ss.StartTime), MONTH(ss.StartTime)) AS a " +
                        "PIVOT(" +
                        "SUM(AvgSession) " +
                        "FOR [Month] IN ([1], [2], [3] , [4], [5], [6], [7], [8], [9], [10], [11], [12])) AS b ORDER BY StackName";

            var command = new SqlCommand(query, connection);
            command.Parameters.Add("@Year", SqlDbType.Int).Value = year;

            using var reader = await command.ExecuteReaderAsync();

            return await CreateMonthlyAvgScoreReportDTOList(reader);
        }
    }
}
