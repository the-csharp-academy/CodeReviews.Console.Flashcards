namespace Flashcards
{
    internal class Program
    {
        private static void Main()
        {
            Stacks.CreateStack();
            Flashcards.FlashcardsTable();
            StudySessions.SessionsTable();
            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("Select option from menu below");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("0. Exit\n");
                Console.WriteLine("1. View stacks\n");
                Console.WriteLine("2. Add stack\n");
                Console.WriteLine("3. Delete stack\n");
                Console.WriteLine("4. Update stack\n");
                Console.WriteLine("5. Edit flashcard\n");
                Console.WriteLine("6. Add flashcards\n");
                Console.WriteLine("7. View flashcards\n");
                Console.WriteLine("8. Delete flashcards\n");
                Console.WriteLine("9. Update flashcards\n");
                Console.WriteLine("10. Start Session\n");
                Console.WriteLine("11. View Sessions\n");

                Console.WriteLine("----------------------------------");

                string? input = Console.ReadLine();

                int choice;

                bool success = int.TryParse(input, out choice);
                while (!success)
                {
                    Console.WriteLine("Please enter a number, to select menu option");
                    input = Console.ReadLine();
                    success = int.TryParse(input, out choice);
                }
                switch (choice)
                {
                    case 0:
                        Console.Clear();
                        Console.Write("\nGoodbye\n");
                        Environment.Exit(0);
                        break;

                    case 1:
                        Console.Clear();
                        Stacks.ViewStack();
                        break;

                    case 2:
                        Console.Clear();
                        Stacks.NewStack();
                        break;

                    case 3:
                        Console.Clear();
                        Stacks.DeleteStack();
                        break;

                    case 4:
                        Console.Clear();
                        Stacks.UpdateStack();
                        break;

                    case 5:
                        Console.Clear();
                        Flashcards.UpdateFlashcards();
                        break;

                    case 6:
                        Console.Clear();
                        Flashcards.AddFlashcards();
                        break;

                    case 7:
                        Console.Clear();
                        Flashcards.ViewFlashcards();
                        break;

                    case 8:
                        Console.Clear();
                        Flashcards.DeleteFlashcards();
                        break;

                    case 9:
                        Console.Clear();
                        Flashcards.UpdateFlashcards();
                        break;

                    case 10:
                        Console.Clear();
                        StudySessions.StartSession();
                        break;

                    case 11:
                        Console.Clear();
                        StudySessions.ViewSessions();
                        break;
                }

                Console.WriteLine("Press 'enter' to return to menu");

                var userInput = Console.ReadKey();
                while (userInput.Key != ConsoleKey.Enter)
                {
                    Console.WriteLine("\nPress 'enter' to return to menu");
                    userInput = Console.ReadKey();
                }
            }
        }
    }
}