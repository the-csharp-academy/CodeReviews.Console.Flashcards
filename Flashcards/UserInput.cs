namespace Flashcards;

public class UserInput
{
    public string NumberInput(string message)
    {
        Console.WriteLine(message);
        
        String? userInput = "";
        bool validNumber = false;

        do
        {

            userInput = Console.ReadLine();

            validNumber = int.TryParse(userInput, out _);

            if (string.IsNullOrWhiteSpace(userInput) || !validNumber)
            {
                Console.WriteLine("Please enter a valid number");
            }
            else
            {
                validNumber = true;
            }

        } while (!validNumber);

        return userInput!;
    }

    public int YearInput(string message)
    {
        Console.WriteLine(message);
        string? input = "";
        bool validYear = false;
        int year = 0;

        do
        {
            input = Console.ReadLine();

            validYear = int.TryParse(input, out year);

            if (!validYear)
            {
                Console.WriteLine("Please enter a valid year");
            }
            else
            {
                validYear = true;
            }
        }while(!validYear);
        return year!;
    }

    public string StringInput(string message)
    {
        Console.WriteLine(message);

        string? userInput = "";
        bool validString = false;

        do
        {
            userInput = Console.ReadLine();

            validString = !string.IsNullOrWhiteSpace(userInput);


            if (!validString)
            {
                Console.WriteLine("Please enter a valid string");
            }
            else if (userInput?.Length > 100)
            {
                Console.WriteLine("Please make sure the string is 100 chars MAX.");
                validString = false;
            }

        } while (!validString);
        return userInput!;
    }
}