using System.ComponentModel.DataAnnotations;

namespace Flashcards.TerrenceLGee.FlashcardUI.Menus;

public enum FlashcardMenu
{
    [Display(Name = "Add a flashcard to a stack")]
    AddFlashcard,
    [Display(Name = "Update an existing flashcard in a stack")]
    UpdateFlashcard,
    [Display(Name = "Delete an existing flashcard in a stack")]
    DeleteFlashcard,
    [Display(Name = "View all flashcards in a stack")]
    ViewAllFlashcards,
    [Display(Name = "View flashcard in a stack")]
    ViewFlashcard,
    [Display(Name = "Return to the previous menu")]
    Exit
}