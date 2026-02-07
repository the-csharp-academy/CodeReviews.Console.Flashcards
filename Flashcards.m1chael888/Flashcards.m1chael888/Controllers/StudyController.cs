using Flashcards.m1chael888.Models;
using Flashcards.m1chael888.Services;
using Flashcards.m1chael888.Views;
using Spectre.Console;
using static Flashcards.m1chael888.Enums.StudyViewEnums;

namespace Flashcards.m1chael888.Controllers
{
    public class StudyController
    {
        private IStudyView _studyView;
        private IStackView _stackView;
        private ICardView _cardView;
        private IStackService _stackService;
        private ICardService _cardService;
        private IStudyService _studyService;
        public StudyController(IStudyView studyView, IStackView stackView, ICardView cardView, IStackService stackService, ICardService cardService, IStudyService studyService)
        {
            _studyView = studyView;
            _stackView = stackView;
            _cardView = cardView;
            _stackService = stackService;
            _cardService = cardService;
            _studyService = studyService;
        }
        public void HandleStudyMenu()
        {
            Console.Clear();
            var choice = CallStudyMenu();

            switch (choice)
            {
                case StudyMenuOption.ChooseStack:
                    CallChooseStack();
                    break;
                case StudyMenuOption.StudyHistory:
                    CallShowSessionHistory();
                    break;
                case StudyMenuOption.Back:
                    break;
            }
        }

        private void CallShowSessionHistory()
        {
            var sessions = GetSessionList();
            if (sessions.Count > 0)
            {
                _studyView.ShowSessionHistory(sessions);
            }
            else
            {
                AnsiConsole.MarkupLine("[lime]You don't have any previous sessions!! Try studying a stack[/]");
                ReturnToMenu("Press any key to go back");
            }
        }

        private void CallChooseStack()
        {
            var stacks = GetStackList();
            var choice = _stackView.DisplayStackPrompt(stacks, "Which stack would you like to study from??");
            var cards = GetCardList(choice).Shuffle().ToList();
            
            if (cards.Count() > 0)
            {
                StartStudySession(choice, cards);
            }
            else
            {
                AnsiConsole.MarkupLine("[lime]This stack doesnt have any cards yet, create cards in the manage menu[/]");
                ReturnToMenu("Press any key to return");
            }
        }

        private void StartStudySession(StackModel choice, List<CardDto> cards)
        {
            int score = 0;
            int count = 0;
            var scoreString = "";

            bool done = false;
            while (!done)
            {
                foreach (var card in cards)
                {
                    Console.Clear();
                    _studyView.ShowFront(card);
                    var result = _studyView.ShowBack(card);
                    switch (result)
                    {
                        case CardResult.Right:
                            count++;
                            score++;
                            break;
                        case CardResult.Wrong:
                            count++;
                            break;
                        case CardResult.Back:
                            done = true;
                            if (count > 0)
                            {
                                scoreString = $"{score}/{count}";
                                CallSessionCreate(scoreString, choice);
                            }
                            EndSession(scoreString, count);
                            break;
                    }
                }
                done = true;
                scoreString = $"{score}/{count}";
                CallSessionCreate(scoreString, choice);
                EndSession($"{score}/{count}", count);
            }
        }

        private void CallSessionCreate(string score, StackModel choice)
        {
            var session = new SessionModel();
            session.Date = DateTime.Now.ToString("yyyy/MM/dd");
            session.Score = score;
            session.StackId = choice.StackId;

            _studyService.SessionCreate(session);
        }

        private void EndSession(string score, int count)
        {
            _studyView.ShowEnd(score, count);
            HandleStudyMenu();
        }

        private List<StackModel> GetStackList()
        {
            var stacks = _stackService.StacksRead();
            return stacks;
        }

        private List<CardDto> GetCardList(StackModel choice)
        {
            var cards = _cardService.CardsRead(choice);
            return cards;
        }

        private List<SessionModel> GetSessionList()
        {
            var sessions = _studyService.SessionsRead();
            return sessions;
        }

        private StudyMenuOption CallStudyMenu()
        {
            var choice = _studyView.ShowMenu();
            return choice;
        }

        private void ReturnToMenu(string msg)
        {
            AnsiConsole.Status()
                .Spinner(Spinner.Known.Point)
                .SpinnerStyle("white")
                .Start(msg, x =>
                {
                    Console.ReadKey();
                });
            HandleStudyMenu();
        }
    }
}
