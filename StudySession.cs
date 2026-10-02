using Flashcards.Controllers;
using Flashcards.DTOs;
using Spectre.Console;

namespace Flashcards
{
    public class StudySession
    {
        private readonly FlashcardController flashcardController;
        private readonly StudySessionController studySessionController;
        private int score;

        public StudySession(FlashcardController flashcardController, StudySessionController studySessionController)
        {
            this.flashcardController = flashcardController;
            this.studySessionController = studySessionController;
        }

        public void StartGame(CardStackDTO cardStack)
        {
            score = 0;
            var studySession = new CreateStudySessionDTO();
            var flashcards = flashcardController.GetFlashcards(cardStack.Id).ToList();
            if (flashcards.Count == 0)
            {
                UI.PrintMessage("This stack has no flashcards. Add flashcards before studying");
                UI.PrintMessage("Press Enter to return to the study menu");
                Console.ReadLine();
                return;
            }
            UI.PrintMessage("To stop studying, enter E instead of an answer.");
            foreach (var flashcard in flashcards)
            {
                if (!StudyCard(flashcard))
                {
                    UI.PrintMessage($"Study stopped: your score is {score}");
                    studySession = new CreateStudySessionDTO
                    {
                        CardStackId = cardStack.Id,
                        Score = score,
                        Time = DateTime.Now
                    };
                    studySessionController.CreateSession(studySession);
                    return;
                }
            }
            UI.PrintMessage($"Flashcards ended: your score is {score}");
            studySession = new CreateStudySessionDTO
            {
                CardStackId = cardStack.Id,
                Score = score,
                Time = DateTime.Now
            };
            studySessionController.CreateSession(studySession);
            UI.PrintMessage("Press Enter to return to the study menu.");
            Console.ReadLine();
            Console.Clear();
        }


        private bool StudyCard(GetFlashcardDTO flashcard)
        {
            var answer = AnsiConsole.Ask<string>($"{Markup.Escape(flashcard.Front)}: print a translation: ");
            if (answer.Trim().Equals("E", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (answer.Equals(flashcard.Back))
            {
                score++;
                UI.PrintMessage($"Correct, score is {score}");
            }
            else
            {
                UI.PrintMessage($"Incorrect, answer is {flashcard.Back}, score is {score}");
            }
            return true;
        }

    }
}
