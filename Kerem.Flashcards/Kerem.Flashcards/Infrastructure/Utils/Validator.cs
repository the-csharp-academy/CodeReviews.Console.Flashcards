namespace Kerem.Flashcards.Infrastructure.Utils ;

    public static class Validator
    {
        public static bool Abort(string choice)
        {
            if (choice == "abort")
            {
                return true;
            }
            return false;
        }
    }