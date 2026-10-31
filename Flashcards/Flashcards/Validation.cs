
public class Validation
{
    public static int ValidateId(string? input)
    {

        int result;


        while (!int.TryParse(input, out result) || result < 0)
        {
            Console.WriteLine("Please enter a valid ID.");
            input = Console.ReadLine();

        }

        return result;
    }

    public static string GetNonEmptyInput(string prompt)
    {
        Console.WriteLine(prompt);
        string? input = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Input can't be empty. Please try again:");
            input = Console.ReadLine();
        }

        return input.Trim();
    }

    public static int GetExistingStackId()
    {
        Console.WriteLine("Enter the ID of the stack or enter 0 to return: ");
        int id = Validation.ValidateId(Console.ReadLine());

        while (id != 0 && !DatabaseController.StackExists(id))
        {
            Console.WriteLine("No stack with that ID. Please try again or enter 0 to return: ");
            id = Validation.ValidateId(Console.ReadLine());
        }

        return id;
    }

    // private static int GetRealFlashcardId(string prompt)
    // {
    //     List<Flashcard> cards = DatabaseController.GetFlashcardsWithIds(stackId);

    //     Console.WriteLine(prompt);
    //     int number = Validation.ValidateId(Console.ReadLine());

    //     while (number > cards.Count)
    //     {
    //         Console.WriteLine($"There are only {cards.Count} flashcards. Try again or enter 0 to return:");
    //         number = Validation.ValidateId(Console.ReadLine());
    //     }

    //     if (number == 0) return 0;

    //     return cards[number - 1].Id;
    // }



}