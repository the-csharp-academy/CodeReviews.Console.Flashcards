using Flashcards.Entities;

namespace Flashcards.Validation
{
    public static class StackValidator
    {
        public static async Task<bool> ValidateStack(Stack stack)
        {
            if (string.IsNullOrWhiteSpace(stack.Name))
            {
                return false;
            }

            return true;
        }
    }
}
