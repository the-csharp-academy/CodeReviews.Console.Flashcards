namespace Flashcards;

internal record FlaschcardDto(int DisplayId, string Recto, string Verso);

internal record StackDto(int DisplayId, string Name);

internal record StudyDto(DateTime Date, string Name, int Score, int ScoreMax)
{
    private int _percentage;
    internal int Percentage
    {
        get { return _percentage; }
        private set
        {
            if (ScoreMax != 0) { _percentage = Score * 100 / ScoreMax; }
            else { _percentage = 0; }
        }
    }

}