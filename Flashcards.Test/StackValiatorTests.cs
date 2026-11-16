using Flashcards.Entities;
using Flashcards.Validation;

namespace Flashcards.Test
{
    public class StackValidatorTests
    {
        [Fact]
        public async Task StackIsInvalid_WhenNameIsEmpty()
        {
            var stack = new Stack
            {
                Id = 1,
                Name = ""
            };

            var result = await StackValidator.ValidateStack(stack);

            Assert.False(result);
        }
    }
}
