namespace Flashcards
{
    internal class Helpers
    {
        internal int CheckIntInput()
        {
            int number = 0;
            bool isValid = false;

            do 
            { 
                string input = Console.ReadLine();
                input = input.Trim().ToLower();
                isValid = int.TryParse(input, out number);

                if (!isValid)
                    Console.WriteLine("\nPlease input a number!");

            } while(!isValid);

            return number;
        }
    }
}
