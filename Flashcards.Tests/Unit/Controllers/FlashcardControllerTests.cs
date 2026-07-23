using System.Reflection;
using CodeReviews.Console.Flashcards;
using NSubstitute;
using NUnit.Framework;

namespace CodeReviews.Console.Flashcards.Tests;

[TestFixture]
[Category("Unit")]
public sealed class FlashcardControllerTests
{
    private IFlashcardsView _view;
    private IFlashcardsRepo _flashcardsRepo;
    private IStacksRepo _stacksRepo;
    private IFlashcardController _controller;

    [SetUp]
    public void SetUp()
    {
        _view = Substitute.For<IFlashcardsView>();
        _flashcardsRepo = Substitute.For<IFlashcardsRepo>();
        _stacksRepo = Substitute.For<IStacksRepo>();

        _controller = new FlashcardController((IFlashcardsView)_view, _flashcardsRepo, _stacksRepo);
    }

    [Test]
    public void EditCard_WhenStackAndCardExist_UpdatesSelectedFlashcard()
    {
        SetCurrentStackName("German");

        _stacksRepo.GetStackByName("German").Returns(new CardStack
        {
            StackId = 10,
            Name = "German"
        });

        _flashcardsRepo.GetAllByStackId(10).Returns(new List<Flashcard>
        {
            new()
            {
                FlashcardId = 5,
                StackId = 10,
                Question = "Old question",
                Answer = "Old answer"
            }
        });

        _view.AskFlashcardIndex(1).Returns(1);
        _view.AskFlashcardContent().Returns(("Updated question", "Updated answer"));

        _controller.EditCard();

        _flashcardsRepo.Received(1).Update(Arg.Is<Flashcard>(card =>
            card.FlashcardId == 5 &&
            card.StackId == 10 &&
            card.Question == "Updated question" &&
            card.Answer == "Updated answer"));

        _view.Received(1).DisplayFlashcards(Arg.Any<IReadOnlyList<FlashcardDTO>>());
    }

    private static void SetCurrentStackName(object controller, string stackName)
    {
        FieldInfo? field = controller.GetType().GetField("_currentStackName", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, "Current stack field should exist.");
        field!.SetValue(controller, stackName);
    }

    private void SetCurrentStackName(string stackName) => SetCurrentStackName(_controller, stackName);
}
