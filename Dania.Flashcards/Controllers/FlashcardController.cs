using Flashcards.Models;

namespace Flashcards.Controllers
{
    internal class FlashcardController
    {
        List<FlashcardDto> flashcards = new List<FlashcardDto>();
        DatabaseManager databaseManager;
        Helpers helpers = new Helpers();

        public FlashcardController(DatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        internal void ViewFlashcards(Stack stack)
        {
            GetFlashcards(stack);

            Console.WriteLine("\nPress Enter to return to the manage flashcard menu...");
            Console.ReadLine();
        }

        internal void CreateFlashcards(Stack stack)
        {
            Console.Clear();
            Console.WriteLine($"---Create flashcards in Stack: {stack.Name}---");

            Console.WriteLine("Enter the question for the flashcard: ");
            string question = Console.ReadLine();

            if (question == "0") return;

            Console.WriteLine("\nEnter the answer for the flashcard: ");
            string answer = Console.ReadLine();

            if (answer == "0") return;

            databaseManager.CreateFlashcard(stack.Id, question, answer);
            Console.WriteLine("\nFlashcard created successfully!");

            Console.Write("Would you like to create another flashcard? (y/n): ");
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input) && input.ToLower() == "y")
                CreateFlashcards(stack);
            else
            {
                Console.WriteLine("\nPress Enter to return to the manage flashcard menu...");
                Console.ReadLine();
            }
        }

        internal void UpdateFlashcards(Stack stack)
        {
            Console.Clear();
            Console.WriteLine("---Update flashcards---");
            GetFlashcards(stack);
            if (flashcards.Count > 0)
            {
                Console.WriteLine("\nEnter the flashcard id to update: ");
                int flashcardDisplayId = helpers.CheckIntInput();

                if (flashcardDisplayId == 0) return;

                var flashcard = flashcards.FirstOrDefault(f => f.DisplayId == flashcardDisplayId);

                if (flashcard == null)
                    Console.WriteLine($"Flashcard with ID '{flashcardDisplayId}' does not exist. Please try again.");
                else
                {
                    Console.WriteLine($"\nCurrent question: {flashcard.Question}");
                    Console.WriteLine("Enter the new question for the flashcard (Press Enter to keep the current question): ");
                    string newQuestion = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newQuestion))
                        flashcard.Question = newQuestion;

                    Console.WriteLine($"\nCurrent answer: {flashcard.Answer}");
                    Console.WriteLine("Enter the new answer for the flashcard (Press Enter to keep the current answer): ");
                    string newAnswer = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newAnswer))
                        flashcard.Answer = newAnswer;

                    databaseManager.UpdateFlashcard(flashcard.Id, flashcard.Question, flashcard.Answer);
                    Console.WriteLine($"\nFlashcard with ID '{flashcardDisplayId}' updated successfully!");
                }

                Console.WriteLine("\nWould you like to update another flashcard? (y/n): ");
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input) && input.ToLower() == "y")
                    UpdateFlashcards(stack);
                else
                {
                    Console.WriteLine("\nPress Enter to return to the manage flashcard menu...");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("\nPress Enter to return to the manage flashcard menu...");
                Console.ReadLine();
            }
        }

        internal void DeleteFlashcards(Stack stack)
        {
            Console.Clear();
            Console.WriteLine("---Delete flashcards---");

            GetFlashcards(stack);
            if (flashcards.Count > 0)
            {
                Console.WriteLine("\nEnter the flashcard id to delete: ");
                int flashcardDisplayId = helpers.CheckIntInput();

                if (flashcardDisplayId == 0) return;

                // Find the flashcard by its display ID
                var flashcard = flashcards.FirstOrDefault(f => f.DisplayId == flashcardDisplayId);

                if (flashcard == null)
                    Console.WriteLine($"Flashcard with ID '{flashcardDisplayId}' does not exist. Please try again.");
                else
                {
                    Console.WriteLine($"\nAre you sure you want to delete flashcard with ID '{flashcardDisplayId}'? (y/n): ");
                    string confirmation = Console.ReadLine();

                    if (confirmation.Trim().ToLower() == "y")
                    {
                        databaseManager.DeleteFlashcard(flashcard.Id);
                        Console.WriteLine($"\nFlashcard with ID '{flashcardDisplayId}' deleted successfully!");
                    }
                    else
                        Console.WriteLine("\nDeletion cancelled.");

                }

                Console.WriteLine("\nWould you like to delete another flashcard? (y/n): ");
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input) && input.ToLower() == "y")
                    DeleteFlashcards(stack);
                else
                {
                    Console.WriteLine("\nPress Enter to return to the manage flashcard menu...");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("\nPress Enter to return to the manage flashcard menu...");
                Console.ReadLine();
            }
        }



        internal void GetFlashcards(Stack stack)
        {
            flashcards.Clear();
            flashcards = databaseManager.GetFlashcardsByStackId(stack.Id);

            Console.Clear();
            Console.WriteLine($"---Flashcards in Stack: {stack.Name}---");
            if (flashcards.Count == 0)
            {
                Console.WriteLine("No flashcards found in this stack.");
            }
            else
            {
                Console.WriteLine("{0,-7} {1,-15} {2,-15}", "ID", "Question", "Answer");
                foreach (var flashcard in flashcards)
                {
                    Console.WriteLine("{0,-7} {1,-15} {2,-15}", flashcard.DisplayId, flashcard.Question, flashcard.Answer);

                }
            }
        }
    }
}
