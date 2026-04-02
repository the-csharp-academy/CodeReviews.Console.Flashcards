namespace Flashcards
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //In case of any issues with encoding, this will ensure that the console can handle UTF-8 characters properly.
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            DatabaseManager databaseManager = new DatabaseManager();
            UserInterface userInterface = new UserInterface();

            databaseManager.CreateTable();
            userInterface.MainMenu(databaseManager);
        }
    }
}
