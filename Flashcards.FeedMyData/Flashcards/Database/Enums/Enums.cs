using System.ComponentModel.DataAnnotations;

namespace Flashcards;

internal class Enums
{
    internal enum MainMenuChoice
    {
        [Display(Name = "[MediumPurple]Study[/]")]
        study,

        [Display(Name = "[MediumPurple]View study[/] sessions")]
        studyView,

        [Display(Name = "[yellow]Manage[/] Stacks")]
        manageStacks,

        [Display(Name = "[yellow]Manage[/] Flashcards")]
        manageFlashcards,

        [Display(Name = "Exit")]
        exit,
    }

    internal enum CrudChoice
    {
        [Display(Name = "[yellow]View[/] items")]
        viewItem,

        [Display(Name = "[green]Add[/] items")]
        addItem,

        [Display(Name = "[blue]Edit[/] items")]
        editItem,

        [Display(Name = "[red]Delete[/] items")]
        deleteItem,

        [Display(Name = "Go back to Main Menu")]
        mainMenu,
    }

    internal enum ObjectType
    {
        flashcard,
        stack,
        study
    }
}