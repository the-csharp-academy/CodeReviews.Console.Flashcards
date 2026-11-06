
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

    public static string GetYesOrNo()
    {
        string answer = (Console.ReadLine() ?? "").ToLower().Trim();

        while (answer != "y" && answer != "n")
        {
            Console.WriteLine("Please enter Y or N.");
            answer = Console.ReadLine().ToLower().Trim();
        }

        return answer;
    }
}