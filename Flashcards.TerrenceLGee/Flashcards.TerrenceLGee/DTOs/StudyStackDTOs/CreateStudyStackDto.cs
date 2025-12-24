using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.DTOs.StudyStackDTOs;

public class CreateStudyStackDto
{
    public Subject Subject { get; set; }
    public string Name { get; set; } = string.Empty;
}