using Microsoft.Data.SqlClient;

namespace Flashcards;

public sealed class StudyRepository(string connectionString)
{
    public async Task AddSessionAsync(int stackId, int score, int totalCards)
    {
        await using var connection = await OpenAsync();
        await using var command = new SqlCommand("""
            INSERT dbo.StudySessions (StackId, StudiedAt, Score, TotalCards)
            VALUES (@stackId, @studiedAt, @score, @totalCards)
            """, connection);
        command.Parameters.AddWithValue("@stackId", stackId);
        command.Parameters.AddWithValue("@studiedAt", DateTime.Now);
        command.Parameters.AddWithValue("@score", score);
        command.Parameters.AddWithValue("@totalCards", totalCards);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<IReadOnlyList<StudySessionDto>> GetSessionsAsync()
    {
        var result = new List<StudySessionDto>();
        await using var connection = await OpenAsync();
        await using var command = new SqlCommand("""
            SELECT ss.Id, s.Name, ss.StudiedAt, ss.Score, ss.TotalCards
            FROM dbo.StudySessions ss JOIN dbo.Stacks s ON s.Id = ss.StackId
            ORDER BY ss.StudiedAt DESC
            """, connection);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetDateTime(2), reader.GetInt32(3), reader.GetInt32(4)));
        return result;
    }

    public Task<IReadOnlyList<MonthlyReportRow>> GetSessionCountReportAsync(int year) =>
        GetReportAsync(year, "COUNT(SessionId)");

    public Task<IReadOnlyList<MonthlyReportRow>> GetAverageScoreReportAsync(int year) =>
        GetReportAsync(year, "AVG(Percentage)");

    private async Task<IReadOnlyList<MonthlyReportRow>> GetReportAsync(int year, string aggregate)
    {
        var result = new List<MonthlyReportRow>();
        await using var connection = await OpenAsync();
        var sql = $$"""
            SELECT StackName, [1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12]
            FROM (
                SELECT s.Name AS StackName, m.MonthNumber, ss.Id AS SessionId,
                       CAST(CASE WHEN ss.TotalCards = 0 THEN 0
                            ELSE ss.Score * 100.0 / ss.TotalCards END AS decimal(10,2)) AS Percentage
                FROM dbo.Stacks s
                CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12)) m(MonthNumber)
                LEFT JOIN dbo.StudySessions ss ON ss.StackId = s.Id
                    AND YEAR(ss.StudiedAt) = @year AND MONTH(ss.StudiedAt) = m.MonthNumber
            ) source
            PIVOT ({{aggregate}} FOR MonthNumber IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])) p
            ORDER BY StackName;
            """;
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@year", year);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var months = Enumerable.Range(1, 12)
                .Select(i => reader.IsDBNull(i) ? 0m : Convert.ToDecimal(reader.GetValue(i))).ToArray();
            result.Add(new(reader.GetString(0), months));
        }
        return result;
    }

    private async Task<SqlConnection> OpenAsync()
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
