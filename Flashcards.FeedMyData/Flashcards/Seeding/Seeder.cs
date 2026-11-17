namespace Flashcards;

internal record CountriesInfo(string Continent, string Country, string Capital);

internal static class Seeder
{
    private static string csvPath = Path.Combine(
                                        AppContext.BaseDirectory,
                                        @"Seeding\countries.csv");

    private static readonly string[] stacks = {
                "Country to Continents",
                "Capitals to Country",
                "Country to Capital"
                };

    internal static void Seed(Database database, Action<int, int> reportProgress)
    {
        CreateStacks(database);
        FillStacks(database, reportProgress);
    }

    private static void FillStacks(Database database, Action<int, int> reportProgress)
    {
        List<CountriesInfo> countries = CsvReader();
        Random rand = new();
        int countriesAdded = 0;

        for (int i = 0; i < countries.Count; i++)
        {
            CountriesInfo country = countries[i];
            int diceRoll = rand.Next(1, 20);


            if (diceRoll == 6)
            {
                database.AddFlashcard(1, country.Country, country.Continent);
                database.AddFlashcard(2, country.Capital, country.Country);
                database.AddFlashcard(3, country.Country, country.Capital);
            }

            reportProgress(i + 1, countries.Count);
        }
    }

    private static void CreateStacks(Database database)
    {
        foreach (string stack in stacks)
            database.AddStack(stack);
    }

    private static List<CountriesInfo> CsvReader()
    {
        return File.ReadLines(csvPath)
            .Select(Line =>
            {
                string[] csvColumn = Line.Split(',');
                return new CountriesInfo(csvColumn[0], csvColumn[1], csvColumn[2]);
            })
            .ToList();
    }
}