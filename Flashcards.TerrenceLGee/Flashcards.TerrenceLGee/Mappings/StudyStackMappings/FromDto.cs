using Flashcards.TerrenceLGee.DTOs.StudyStackDTOs;
using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.Mappings.StudyStackMappings;

public static class FromDto
{
    extension(CreateStudyStackDto dto)
    {
        public StudyStack FromCreateStudyStackDto()
        {
            return new StudyStack
            {
                Subject = dto.Subject,
                Name = dto.Name
            };
        }
    }

    extension(UpdateStudyStackDto dto)
    {
        public StudyStack FromUpdateStudyStackDto()
        {
            return new StudyStack
            {
                Id = dto.Id,
                Subject = dto.Subject,
                Name = dto.Name
            };
        }
    }
}