class Program
{
    static void Main(string[] args)
    {

        DatabaseController.CreateStackTable();
        DatabaseController.CreateFlashcardTable();
        DatabaseController.CreateStudySessionsTable();
        Input.GetUserInput();
        Console.ReadLine();

    }
}