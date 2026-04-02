using Flashcards.Models;

namespace Flashcards.Controllers
{
    internal class StackController
    {
        List<Stack> stacks = new List<Stack>();
        DatabaseManager databaseManager;

        public StackController(DatabaseManager databaseManager) 
        { 
            this.databaseManager = databaseManager;
        }

        internal void CreateStack()
        {
            Console.Clear();
            Console.WriteLine("---Create a new Stack---");

            Console.WriteLine("Enter the name of the new stack (Press 0 to return to the manage stacks menu): ");
            string name = Console.ReadLine();

            if (name == "0") return;

            Console.WriteLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Stack name cannot be empty. Please try again.");
            }
            else
            {
                if (databaseManager.CheckStackExist(name))
                    Console.WriteLine($"Stack '{name}' already exists. Please choose a different name.");
                else
                {
                    databaseManager.CreateStack(name);
                    Console.WriteLine($"Stack '{name}' created successfully!");
                }
            }

            Console.WriteLine("\nWould you like to create another stack? (y/n): ");
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input) && input.ToLower() == "y")
                CreateStack();
            else
            {
                Console.WriteLine("\nPress Enter to return to the manage stacks menu...");
                Console.ReadLine();
            }
        }

        internal void DeleteStack()
        {
            Console.Clear();
            Console.WriteLine("---Delete Stacks---");

            if (GetStacks() > 0)
            {
                Console.WriteLine("\nEnter the stack name to delete (Press 0 to return to the manage stacks menu):");
                string stackName = Console.ReadLine();

                if (stackName == "0") return;

                Stack stack = GetStackByName(stackName);
                if (stack != null)
                {
                    Console.WriteLine($"\nAre you sure you want to delete stack '{stackName}' and all its flashcards? (y/n): ");
                    string confirmation = Console.ReadLine();

                    if (confirmation.Trim().ToLower() == "y")
                    {
                        databaseManager.DeleteStack(stack.Id);
                        Console.WriteLine($"\nStack '{stackName}' and all its flashcards deleted successfully!");
                    }
                    else
                        Console.WriteLine("\nDeletion cancelled.");
                }

                Console.WriteLine("\nWould you like to delete another stack? (y/n): ");
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input) && input.ToLower() == "y")
                    DeleteStack();
                else
                {
                    Console.WriteLine("\nPress Enter to return to the manage stack menu...");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("\nPress Enter to return to the manage stack menu...");
                Console.ReadLine();
            }
        }

        internal int GetStacks()
        {
            stacks.Clear();
            stacks = databaseManager.GetStacks();

            if (stacks.Count == 0)
                Console.WriteLine("No stacks found. Please create a stack first.");
            else
            {
                foreach (var stack in stacks)
                {
                    Console.WriteLine($"- {stack.Name}");
                }
            }

            return stacks.Count;
        }

        internal Stack GetStackByName(string stackName)
        {
            // Use case-insensitive comparison to find the stack by name
            var stack = stacks.FirstOrDefault(s => s.Name.Equals(stackName, StringComparison.OrdinalIgnoreCase));

            if (stack == null)
                Console.WriteLine($"\nStack '{stackName}' does not exist. Please try again.");

            return stack;
        }
    }
}
