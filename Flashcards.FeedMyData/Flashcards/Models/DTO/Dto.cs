namespace Flashcards;

internal record FlaschcardDto(int DisplayId, string Recto, string Verso);

internal record StackDto(int DisplayId, string Name);

internal record StudyDto(DateTime Date, string Name, int Score, int ScoreMax)
{
    internal int Percentage => ScoreMax == 0
        ? 0
        : Score * 100 / ScoreMax;

}