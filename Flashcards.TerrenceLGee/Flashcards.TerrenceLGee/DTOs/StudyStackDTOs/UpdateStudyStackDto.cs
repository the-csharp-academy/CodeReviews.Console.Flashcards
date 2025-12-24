using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.DTOs.StudyStackDTOs;

public class UpdateStudyStackDto
{
    public int Id { get; set; }
    public Subject Subject { get; set; }
    public string Name { get; set; } = string.Empty;
}