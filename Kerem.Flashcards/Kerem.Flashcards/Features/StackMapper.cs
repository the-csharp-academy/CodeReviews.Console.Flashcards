using Kerem.Flashcards.Domain.Entities;

namespace Kerem.Flashcards.Features ;

    public static class StackMapper
    {
        public static StackDTO MapToDto(Stack stack)
        {
            return new StackDTO()
            {
                Name = stack.Name,
            };
        }
    }