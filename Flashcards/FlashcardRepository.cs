using Microsoft.Data.SqlClient;

namespace Flashcards;

public sealed class FlashcardRepository(string connectionString)
{
    public async Task<IReadOnlyList<Stack>> GetStacksAsync()
    {
        var result = new List<Stack>();
        await using var connection = await OpenAsync();
        await using var command = new SqlCommand("SELECT Id, Name FROM dbo.Stacks ORDER BY Name", connection);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) result.Add(new(reader.GetInt32(0), reader.GetString(1)));
        return result;
    }

    public async Task<Stack?> FindStackAsync(string name)
    {
        await using var connection = await OpenAsync();
        await using var command = new SqlCommand(
            "SELECT Id, Name FROM dbo.Stacks WHERE Name = @name", connection);
        command.Parameters.AddWithValue("@name", name.Trim());
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? new(reader.GetInt32(0), reader.GetString(1)) : null;
    }

    public async Task CreateStackAsync(string name)
    {
        await using var connection = await OpenAsync();
        await using var command = new SqlCommand("INSERT dbo.Stacks (Name) VALUES (@name)", connection);
        command.Parameters.AddWithValue("@name", name.Trim());
        await command.ExecuteNonQueryAsync();
    }

    public async Task RenameStackAsync(int id, string name)
    {
        await using var connection = await OpenAsync();
        await using var command = new SqlCommand("UPDATE dbo.Stacks SET Name = @name WHERE Id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@name", name.Trim());
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteStackAsync(int id)
    {
        await using var connection = await OpenAsync();
        await using var command = new SqlCommand("DELETE dbo.Stacks WHERE Id = @id", connection);
        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<IReadOnlyList<FlashcardDto>> GetCardsAsync(int stackId, int? take = null)
    {
        var result = new List<FlashcardDto>();
        await using var connection = await OpenAsync();
        const string sql = """
            WITH Numbered AS (
                SELECT ROW_NUMBER() OVER (ORDER BY Id) AS DisplayId, Front, Back
                FROM dbo.Flashcards WHERE StackId = @stackId
            )
            SELECT DisplayId, Front, Back FROM Numbered
            WHERE @take IS NULL OR DisplayId <= @take ORDER BY DisplayId;
            """;
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@stackId", stackId);
        command.Parameters.AddWithValue("@take", (object?)take ?? DBNull.Value);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(new(Convert.ToInt32(reader.GetInt64(0)), reader.GetString(1), reader.GetString(2)));
        return result;
    }

    public async Task AddCardAsync(int stackId, string front, string back)
    {
        await using var connection = await OpenAsync();
        await using var command = new SqlCommand(
            "INSERT dbo.Flashcards (StackId, Front, Back) VALUES (@stackId, @front, @back)", connection);
        command.Parameters.AddWithValue("@stackId", stackId);
        command.Parameters.AddWithValue("@front", front.Trim());
        command.Parameters.AddWithValue("@back", back.Trim());
        await command.ExecuteNonQueryAsync();
    }

    public Task<bool> UpdateCardAsync(int stackId, int displayId, string front, string back) =>
        ChangeCardAsync(stackId, displayId, "UPDATE dbo.Flashcards SET Front = @front, Back = @back WHERE Id = @id", front, back);

    public Task<bool> DeleteCardAsync(int stackId, int displayId) =>
        ChangeCardAsync(stackId, displayId, "DELETE dbo.Flashcards WHERE Id = @id", null, null);

    private async Task<bool> ChangeCardAsync(int stackId, int displayId, string action, string? front, string? back)
    {
        await using var connection = await OpenAsync();
        const string findSql = """
            SELECT Id FROM (
                SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS DisplayId
                FROM dbo.Flashcards WHERE StackId = @stackId
            ) numbered WHERE DisplayId = @displayId;
            """;
        await using var find = new SqlCommand(findSql, connection);
        find.Parameters.AddWithValue("@stackId", stackId);
        find.Parameters.AddWithValue("@displayId", displayId);
        var id = await find.ExecuteScalarAsync();
        if (id is null) return false;
        await using var command = new SqlCommand(action, connection);
        command.Parameters.AddWithValue("@id", id);
        if (front is not null) command.Parameters.AddWithValue("@front", front.Trim());
        if (back is not null) command.Parameters.AddWithValue("@back", back.Trim());
        return await command.ExecuteNonQueryAsync() == 1;
    }

    private async Task<SqlConnection> OpenAsync()
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
