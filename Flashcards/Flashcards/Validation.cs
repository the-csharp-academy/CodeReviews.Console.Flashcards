using Microsoft.IdentityModel.Tokens.Experimental;
using Microsoft.VisualBasic;

public class Validation
{
    // input validation logic
    public static int ValidateId(string Input)
    {

        int result;

        
        while (!int.TryParse(Input, out result) || result <= 0)
        {
            Console.WriteLine("Please enter a valid ID.");
            Input = Console.ReadLine();

        }

        return result;
    }






}