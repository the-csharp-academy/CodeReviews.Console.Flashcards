using System.ComponentModel.DataAnnotations;

namespace Flashcards.TerrenceLGee.FlashcardUI.Menus;

public enum StudySessionMenu
{
    [Display(Name = "Start a new study session")]
    StartStudySession,
    [Display(Name = "View a previous study session")]
    ViewPreviousStudySession,
    [Display(Name = "View previous study sessions")]
    ViewPreviousStudySessions,
    [Display(Name = "Return to the previous menu")]
    Exit
}