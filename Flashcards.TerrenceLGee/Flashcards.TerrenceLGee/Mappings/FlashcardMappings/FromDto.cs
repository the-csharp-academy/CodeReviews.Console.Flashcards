using Flashcards.TerrenceLGee.DTOs.FlashcardDTOs;
using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.Mappings.FlashcardMappings;

public static class FromDto
{
    extension(CreateFlashcardDto dto)
    {
        public Flashcard FromCreateFlashcardDto()
        {
            return new Flashcard
            {
                StackId = dto.StackId,
                Question = dto.Question,
                Answer = dto.Answer,
                Position = dto.Position
            };
        }
    }

    extension(UpdateFlashcardDto dto)
    {
        public Flashcard FromUpdateFlashcardDto()
        {
            return new Flashcard
            {
                Id = dto.Id,
                StackId = dto.StackId,
                Question = dto.Question,
                Answer = dto.Answer,
                Position = dto.Position
            };
        }
    }

    extension(RetrievedFlashcardDto dto)
    {
        public UpdateFlashcardDto FromRetrievedFlashcardDtoToUpdateFlashcardDto()
        {
            return new UpdateFlashcardDto
            {
                Id = dto.Id,
                StackId = dto.StackId,
                Question = dto.Question,
                Answer = dto.Answer,
                Position = dto.Position
            };
        }
    }
}