using silvermax.FlashCards;

internal class Program
{
    private static void Main(string[] args)
    {
        UserInterface ui = new();
        DataSeeder seeder = new();

        seeder.Seed();
        ui.Start();
        
    }
}