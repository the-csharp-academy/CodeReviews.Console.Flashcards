using Spectre.Console;

namespace silvermax.FlashCards;

internal class UserInput
{
    public (string word, string translation) GetUserInput()
    {
        var word = AnsiConsole.Ask<string>("Please enter the word: ");
        var translation = AnsiConsole.Ask<string>("Please enter the translation: ");

        return (word, translation);
    }
}
