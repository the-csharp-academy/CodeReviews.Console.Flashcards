using System.ComponentModel.DataAnnotations;

namespace Flashcards.TerrenceLGee.FlashcardUI.Menus;

public enum StackMenu
{
    [Display(Name = "Add a new stack")]
    AddStack,
    [Display(Name = "Update a stack")]
    UpdateStack,
    [Display(Name = "Delete a stack")]
    DeleteStack,
    [Display(Name = "View all stacks")]
    ViewStacks,
    [Display(Name = "View a stack")]
    ViewStack,
    [Display(Name = "Go to flash cards")]
    GoToFlashcardMenu,
    [Display(Name = "Go to study session")]
    GoToStudySessionMenu,
    [Display(Name = "Exit")]
    Exit
}