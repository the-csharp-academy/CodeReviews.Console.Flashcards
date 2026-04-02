using Flashcards.Controllers;
using Flashcards.Models;

namespace Flashcards
{
    internal class UserInterface
    {
        StackController stackController = null;
        FlashcardController flashcardController = null;
        StudySessionController studySessionController = null;

        internal void MainMenu(DatabaseManager databaseManager)
        {
            stackController = new StackController(databaseManager);
            flashcardController = new FlashcardController(databaseManager);
            studySessionController = new StudySessionController(databaseManager, stackController);

            bool isCloseApp = false;

            while (!isCloseApp)
            {
                Console.Clear();
                Console.WriteLine("---Welcome to Flashcards!---");
                Console.WriteLine("1 - Manage Stacks");
                Console.WriteLine("2 - Manage Flashcards");
                Console.WriteLine("3 - Study Room");
                Console.WriteLine("0 - Exit");
                Console.Write("Please select an option: ");
                
                string input = Console.ReadLine();

                switch (input)
                {
                    case "0":
                        isCloseApp = true;
                        break;
                    case "1":
                        ManageStacks();
                        break;
                    case "2":
                        ManageFlashcards();
                        break;
                    case "3":
                        StudySession();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ReadLine();
                        break;
                }
            }
        
        }

        private void ManageStacks()
        {
            bool isCloseManageStacks = false;

            while (!isCloseManageStacks)
            {
                Console.Clear();
                Console.WriteLine("---Manage Stacks---");
                Console.WriteLine("1 - Create stacks");
                Console.WriteLine("2 - Delete stacks");
                Console.WriteLine("0 - Return to main menu");
                Console.Write("Please select an option: ");

                string input = Console.ReadLine();

                switch (input)
                { 
                    case "0":
                        isCloseManageStacks = true;
                        break;
                    case "1":
                        stackController.CreateStack();
                        break;
                    case "2":
                        stackController.DeleteStack();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ReadLine();
                        break;
                }

            }
        }

        private void ManageFlashcards()
        {
            Stack currentStack = null;

            Console.Clear();
            Console.WriteLine("---Manage Flashcards---");

            if (stackController.GetStacks() > 0)
            {
                Console.WriteLine("\nEnter the stack name to manage (Press 0 to return to the main menu):");
                string stackName = Console.ReadLine();

                if (stackName == "0") return;

                currentStack = stackController.GetStackByName(stackName);

                if (currentStack == null)
                {
                    Console.WriteLine("\nPress Enter to return to the main menu...");
                    Console.ReadLine();
                }
            }
            else 
            { 
                Console.WriteLine("\nPress Enter to return to the main menu...");
                Console.ReadLine();
            }

            if (currentStack != null)
            { 
                bool isCloseManageFlashcards = false;
                while (!isCloseManageFlashcards)
                { 
                    Console.Clear();
                    Console.WriteLine($"---Manage Flashcards for Stack: {currentStack.Name}---");
                    Console.WriteLine("1 - View flashcards");
                    Console.WriteLine("2 - Create flashcards");
                    Console.WriteLine("3 - Update flashcards");
                    Console.WriteLine("4 - Delete flashcards");
                    Console.WriteLine("0 - Return to main menu");
                    Console.Write("Please select an option: ");

                    string input = Console.ReadLine();

                    switch(input)
                    {
                        case "0":
                            isCloseManageFlashcards = true;
                            break;
                        case "1":
                            flashcardController.ViewFlashcards(currentStack);
                            break;
                        case "2":
                            flashcardController.CreateFlashcards(currentStack);
                            break;
                        case "3":
                            flashcardController.UpdateFlashcards(currentStack);
                            break;
                        case "4":
                            flashcardController.DeleteFlashcards(currentStack);
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            Console.ReadLine();
                            break;
                    }

                }

            }
        }

        private void StudySession()
        {
            bool isCloseStudySession = false;

            while (!isCloseStudySession)
            {
                Console.Clear();
                Console.WriteLine("---Study Room---");
                Console.WriteLine("1 - Start a study session");
                Console.WriteLine("2 - View study session records");
                Console.WriteLine("0 - Return to main menu");
                Console.Write("Please select an option: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "0":
                        isCloseStudySession = true;
                        break;
                    case "1":
                        studySessionController.PrepareStudySession();
                        break;
                    case "2":
                        studySessionController.StudySessionRecords();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }
    }
}
