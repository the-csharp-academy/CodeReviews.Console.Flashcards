using System;
using FlashcardsApp.Repositories;
using FlashcardsApp.Utilities;

namespace FlashcardsApp
{
    public class Menu
    {
        private readonly string _connectionString;
        private const int Width = 50;
        private readonly string border = "+" + new string('-', Width) + "+";
        private readonly string line = new string('-', Width);

        public Menu(string connectionString)
        {
            _connectionString = connectionString;
            Run();
        }

        public void Run()
        {
            var stackRepo = new StackRepository(_connectionString);
            var flashcardRepo = new FlashcardRepository(_connectionString);
            var sessionRepo = new SessionRepository(_connectionString);

            bool closeApp = false;
            
            while (!closeApp)
            {
                Console.Clear();
                Console.WriteLine("\n" + " FLASHCARD APPLICATION ".PadCenter(Width+2, '='));
                Console.WriteLine(border);
                Console.WriteLine("| 1 | Manage Stacks.".PadRight(Width) + " |");
                Console.WriteLine("| 2 | View study data.".PadRight(Width) + " |");
                Console.WriteLine("| 0 | Exit.".PadRight(Width) + " |");
                Console.WriteLine(border);
                Console.Write("Type a number to choose: ");

                string? inp = Console.ReadLine();
                int c;
                while(!int.TryParse(inp, out c) || c < 0 || c > 2)
                {
                    Console.Write("\nPlease enter a valid number.\nTry again: ");
                    inp = Console.ReadLine();
                }

                switch (c)
                {
                    case 0:
                        Console.WriteLine("\n\n" + line);
                        Console.WriteLine("See you again!");
                        Console.ReadKey();
                        closeApp = true;
                        break;
                    case 1:
                        StackMenu stackMenu = new StackMenu(stackRepo, flashcardRepo, sessionRepo);
                        stackMenu.ShowStackMenu();
                        break;
                    case 2:
                        StudyMenu studyMenu = new StudyMenu(sessionRepo);
                        studyMenu.ShowStudyMenu();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}