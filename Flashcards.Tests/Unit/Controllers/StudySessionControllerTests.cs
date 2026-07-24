using NSubstitute;
using NUnit.Framework;

namespace CodeReviews.Console.Flashcards.Tests;

[TestFixture]
[Category("Unit")]
public sealed class StudySessionControllerTests
{
    private IStudySessionsView _view;
    private IStacksRepo _stacksRepo;
    private IFlashcardsRepo _flashcardsRepo;
    private IStudySessionsRepo _studySessionsRepo;
    private IAnswerChecker _answerChecker;
    private IStudySessionController _controller;

    [SetUp]
    public void SetUp()
    {
        _view = Substitute.For<IStudySessionsView>();
        _stacksRepo = Substitute.For<IStacksRepo>();
        _flashcardsRepo = Substitute.For<IFlashcardsRepo>();
        _studySessionsRepo = Substitute.For<IStudySessionsRepo>();
        _answerChecker = Substitute.For<IAnswerChecker>();

        _controller = new StudySessionController(
            _view,
            _stacksRepo,
            _flashcardsRepo,
            _studySessionsRepo,
            _answerChecker);
    }

    [Test]
    public void Study_WhenStackDoesNotExist_DisplaysErrorAndStops()
    {
        _view.AskForStackName().Returns("Missing");

        _stacksRepo.GetStackByName("Missing").Returns((CardStack?)null);

        _controller.Study();

        _view.Received(1).DisplayError("Stack 'Missing' was not found.");

        _flashcardsRepo.DidNotReceive().GetAllByStackId(Arg.Any<int>());

        _studySessionsRepo.DidNotReceive().Add(Arg.Any<StudySession>());
    }

    [Test]
    public void Study_WhenStackLookupThrows_DisplaysErrorAndStops()
    {
        _view.AskForStackName().Returns("German");
        _stacksRepo.GetStackByName("German").Returns(
            _ => throw new InvalidOperationException("Database unavailable."));

        _controller.Study();

        _view.Received(1).DisplayError(
                "Could not retrieve the stack: " +
                "Database unavailable.");

        _flashcardsRepo.DidNotReceive().GetAllByStackId(Arg.Any<int>());
        _studySessionsRepo.DidNotReceive().Add(Arg.Any<StudySession>());
    }

    [Test]
    public void Study_WhenCardLookupThrows_DisplaysErrorAndStops()
    {
        ConfigureStack("German", 17);

        _flashcardsRepo.GetAllByStackId(17)
            .Returns(_ => throw new InvalidOperationException(
                "Could not load cards."));

        _controller.Study();

        _view.Received(1).DisplayError(
                "Could not retrieve flashcards: " +
                "Could not load cards.");

        _view.DidNotReceive().TakeAnswerFromUser();
        _studySessionsRepo.DidNotReceive().Add(Arg.Any<StudySession>());
    }

    [Test]
    public void Study_WhenStackHasNoCards_DisplaysMessageAndDoesNotSave()
    {
        ConfigureStack("German", 17);

        _flashcardsRepo.GetAllByStackId(17).Returns(Array.Empty<Flashcard>());

        _controller.Study();

        _view.Received(1).DisplayMessage("Stack 'German' has no flashcards.");

        _view.DidNotReceive().TakeAnswerFromUser();
        _studySessionsRepo.DidNotReceive().Add(Arg.Any<StudySession>());
    }

    [Test]
    public void Study_WhenAnswersAreMixed_SavesScoreAndStackSnapshot()
    {
        ConfigureStack("German", 17);

        var cards = new List<Flashcard>
        {
            new()
            {
                FlashcardId = 1,
                StackId = 17,
                Question = "Haus",
                Answer = "House"
            },
            new()
            {
                FlashcardId = 2,
                StackId = 17,
                Question = "Katze",
                Answer = "Cat"
            },
            new()
            {
                FlashcardId = 3,
                StackId = 17,
                Question = "Baum",
                Answer = "Tree"
            }
        };

        _flashcardsRepo.GetAllByStackId(17).Returns(cards);

        _view.TakeAnswerFromUser().Returns("House", "Dog", "Tree");

        _answerChecker.IsCorrect("House", "House").Returns(true);
        _answerChecker.IsCorrect("Dog", "Cat").Returns(false);
        _answerChecker.IsCorrect("Tree", "Tree").Returns(true);

        _controller.Study();

        _studySessionsRepo.Received(1).Add(
                Arg.Is<StudySession>(session =>
                    session != null &&
                    session.StackId == 17 &&
                    session.StackNameSnapshot == "German" &&
                    session.Score == 2 &&
                    session.TotalQuestions == 3));

        _view.Received(1).ShowFinalResult(2, 3);
    }

    [Test]
    public void Study_DisplaysEachQuestionAndItsAnswerResult()
    {
        ConfigureStack("German", 17);

        _flashcardsRepo.GetAllByStackId(17).Returns(
                new List<Flashcard>
                {
                    new()
                    {
                        FlashcardId = 1,
                        StackId = 17,
                        Question = "Haus",
                        Answer = "House"
                    },
                    new()
                    {
                        FlashcardId = 2,
                        StackId = 17,
                        Question = "Baum",
                        Answer = "Tree"
                    }
                });

        _view.TakeAnswerFromUser().Returns("House", "Wrong");

        _answerChecker.IsCorrect("House", "House").Returns(true);
        _answerChecker.IsCorrect("Wrong", "Tree").Returns(false);

        _controller.Study();

        _view.Received(1).ShowQuestion("Haus", 1, 2);
        _view.Received(1).ShowQuestion("Baum", 2, 2);
        _view.Received(1).ShowAnswer(true, "House");
        _view.Received(1).ShowAnswer(false, "Tree");
    }

    [Test]
    public void Study_WhenSavingFails_DisplaysErrorAndDoesNotShowFinalResult()
    {
        ConfigureStack("German", 17);

        _flashcardsRepo.GetAllByStackId(17).Returns(
                new List<Flashcard>
                {
                    new()
                    {
                        FlashcardId = 1,
                        StackId = 17,
                        Question = "Haus",
                        Answer = "House"
                    }
                });

        _view.TakeAnswerFromUser().Returns("House");

        _answerChecker.IsCorrect("House", "House").Returns(true);

        _studySessionsRepo.When(repository =>
                repository.Add(Arg.Any<StudySession>()))
                .Do(_ => throw new InvalidOperationException("Save failed."));

        _controller.Study();

        _view.Received(1)
            .DisplayError(
                "The result could not be saved: " +
                "Save failed.");

        _view.DidNotReceive().ShowFinalResult(Arg.Any<int>(), Arg.Any<int>());
    }

    [Test]
    public void ViewHistory_WhenSessionsExist_DisplaysReturnedSessions()
    {
        IReadOnlyList<StudySessionDTO> sessions =
            new List<StudySessionDTO>
            {
                new()
                {
                    StackName = "German",
                    Score = 8,
                    TotalQuestions = 10,
                    CompletedAt =
                        new DateTime(2026, 7, 24, 10, 0, 0)
                }
            };

        _studySessionsRepo.GetAll().Returns(sessions);

        _controller.ViewHistory();

        _view.Received(1).DisplayAllStudySessions(sessions);
    }

    [Test]
    public void ViewHistory_WhenRepositoryThrows_DisplaysError()
    {
        _studySessionsRepo.GetAll()
            .Returns(_ => throw new InvalidOperationException("History unavailable."));

        _controller.ViewHistory();

        _view.Received(1)
            .DisplayError(
                "Could not retrieve sessions: " +
                "History unavailable.");

        _view.DidNotReceive().DisplayAllStudySessions(Arg.Any<IReadOnlyList<StudySessionDTO>>());
    }

    private void ConfigureStack(string stackName, int stackId)
    {
        _view.AskForStackName().Returns(stackName);

        _stacksRepo.GetStackByName(stackName)
            .Returns(
                new CardStack
                {
                    StackId = stackId,
                    Name = stackName
                });
    }
}