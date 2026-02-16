using System.ComponentModel;

namespace Flashcards.m1chael888.Enums;

public static class ManageEnums
{
    public enum StackMenuOption
    {
        [Description("Create a stack")]
        CreateStack,
        [Description("View stacks")]
        ViewStacks,
        [Description("Update a stack name")]
        UpdateStack,
        [Description("Delete a stack")]
        DeleteStack,
        [Description("Back")]
        Back
    }

    public enum ViewStacksOption
    {
        [Description("View/manage cards in a stack")]
        ViewCards,
        [Description("Back")]
        Back
    }

    public enum ViewCardsOption
    {
        [Description("Create a card")]
        CreateCard,
        [Description("Update a card")]
        UpdateCard,
        [Description("Delete a card")]
        DeleteCard,
        [Description("Back")]
        Back
    }
}
