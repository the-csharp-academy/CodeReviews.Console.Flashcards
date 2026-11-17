namespace Flashcards;

internal static class ModelsMapper
{
    internal static FlaschcardDto FlashcardToDto(Flashcard f, int displayId)
    {
        return new FlaschcardDto(displayId, f.Recto, f.Verso);
    }

    internal static StackDto StackToDto(Stack stack, int displayId)
    {
        return new StackDto(displayId, stack.Name);
    }

    internal static StudyDto StudyToDto(Study study, string stackName)
    {
        return new StudyDto(
            study.Date,
            stackName,
            study.Score,
            study.ScoreMax);
    }

    internal static List<FlaschcardDto> FlashcardToDto(List<Flashcard> flashcards)
    {
        List<FlaschcardDto> flashcardDtos = new();

        for (int i = 0; i < flashcards.Count; i++)
            flashcardDtos.Add(FlashcardToDto(flashcards[i], i + 1));

        return flashcardDtos;
    }

    internal static List<StackDto> StackToDto(List<Stack> stacks)
    {
        List<StackDto> stacksDto = new();

        for (int i = 0; i < stacks.Count; i++)
            stacksDto.Add(StackToDto(stacks[i], i + 1));

        return stacksDto;
    }
}