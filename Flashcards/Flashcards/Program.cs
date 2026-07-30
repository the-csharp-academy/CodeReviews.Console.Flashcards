class Program
{
    // string connectionString= 
    static void Main(string[] args)
    {

        DatabaseController.CreateStackTable();
        DatabaseController.CreateFlashcardTable();
        DatabaseController.CreateStudySessionsTable();
        Input.GetUserInput();
        Console.ReadLine();

    }


}