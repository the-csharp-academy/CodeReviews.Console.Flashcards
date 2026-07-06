using Dapper;
using Microsoft.Data.SqlClient;

namespace silvermax.FlashCards;

internal class DataSeeder
{
    private readonly DbConfig dbConfig = new();
    public void Seed()
    {
        var stacks = new Dictionary<string, List<(string Word, string Translation)>>
        {
            ["portuguese"] = new()
            {
                ("house", "casa"),
                ("dog", "cachorro"),
                ("cat", "gato"),
                ("water", "água"),
                ("food", "comida"),
            },
            ["spanish"] = new()
            {
                ("house", "casa"),
                ("dog", "perro"),
                ("cat", "gato"),
                ("water", "agua"),
                ("food", "comida"),
            },
            ["french"] = new()
            {
                ("house", "maison"),
                ("dog", "chien"),
                ("cat", "chat"),
                ("water", "eau"),
                ("food", "nourriture"),
            },
        };

        using var connection = new SqlConnection(dbConfig.ConnectionString);

        foreach (var stack in stacks)
        {
            connection.Execute("""
                IF NOT EXISTS (SELECT 1 FROM Stacks WHERE StackName = @StackName)
                    INSERT INTO Stacks (StackName) VALUES (@StackName)
                """, new { StackName = stack.Key });

            int stackId = connection.QuerySingle<int>(
                "SELECT Id FROM Stacks WHERE StackName = @StackName",
                new { StackName = stack.Key });

            foreach (var (word, translation) in stack.Value)
            {
                // Only insert if not already present
                connection.Execute("""
                    IF NOT EXISTS (SELECT 1 FROM Flashcards WHERE StackId = @StackId AND Word = @Word)
                        INSERT INTO Flashcards (StackId, Word, Translation) VALUES (@StackId, @Word, @Translation)
                    """, new { StackId = stackId, Word = word, Translation = translation });
            }
        }

        Console.WriteLine("Database seeded successfully!");
        Thread.Sleep(1000);
    }
}
