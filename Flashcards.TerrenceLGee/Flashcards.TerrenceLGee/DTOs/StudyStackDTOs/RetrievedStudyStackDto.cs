using Flashcards.TerrenceLGee.DTOs.FlashcardDTOs;
using Flashcards.TerrenceLGee.DTOs.StudySessionDTOs;
using Flashcards.TerrenceLGee.Models;

namespace Flashcards.TerrenceLGee.DTOs.StudyStackDTOs;

public class RetrievedStudyStackDto
{
    public int Id { get; set; }
    public Subject Subject { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<RetrievedFlashcardDto> Flashcards { get; set; } = [];
    public List<RetrievedStudySessionDto> StudySessions { get; set; } = [];
}