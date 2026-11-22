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
        
        public static bool EmptyName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return true;
            }
            return false;
        }
    }
    
