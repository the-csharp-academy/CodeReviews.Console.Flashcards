using System.ComponentModel;

namespace Flashcards.m1chael888.Enums;

public static class MainMenuViewEnums
{
    public enum MainMenuOption
    {
        [Description("Study")]
        Study,
        [Description("Manage stacks")]
        Manage,
        [Description("Exit")]
        Exit
    }
}
