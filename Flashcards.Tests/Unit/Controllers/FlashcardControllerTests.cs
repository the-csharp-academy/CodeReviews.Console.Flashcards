using System.Reflection;
using NSubstitute;

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

        _controller = new FlashcardController(_view, _flashcardsRepo, _stacksRepo);
    }

    [Test]
    public void AddCard_WhenNoStackIsSelected_DisplaysErrorAndDoesNotAdd()
    {
        _controller.AddCard();

        _view.Received(1).DisplayError("Select a stack first.");

        _flashcardsRepo.DidNotReceive().Add(Arg.Any<Flashcard>());
    }

    [Test]
    public void AddCard_WhenStackDoesNotExist_DisplaysErrorAndDoesNotAdd()
    {
        SetCurrentStackName("Missing");

        _stacksRepo.GetStackByName("Missing").Returns((CardStack?)null);

        _controller.AddCard();

        _view.Received(1).DisplayError("Stack 'Missing' could not be found.");

        _flashcardsRepo.DidNotReceive().Add(Arg.Any<Flashcard>());
    }

    [Test]
    public void AddCard_WhenStackExists_AddsCardWithSelectedStackId()
    {
        ConfigureSelectedStack("German", 17);

        _view.AskFlashcardContent().Returns(("Haus", "House"));

        _controller.AddCard();

        _flashcardsRepo.Received(1).Add(
            Arg.Is<Flashcard>(card =>
                card != null &&
                card.StackId == 17 &&
                card.Question == "Haus" &&
                card.Answer == "House"));
    }

    [Test]
    public void AddCard_WhenRepositoryThrows_DisplaysError()
    {
        SetCurrentStackName("German");

        _stacksRepo.GetStackByName("German")
            .Returns(new CardStack
            {
                StackId = 1,
                Name = "German"
            });

        _view.AskFlashcardContent().Returns(("Question", "Answer"));

        _flashcardsRepo
            .When(repo => repo.Add(Arg.Any<Flashcard>()))
            .Do(_ => throw new Exception("Database error"));

        _controller.AddCard();

        _view.Received(1).DisplayError("Could not add flashcard: Database error");

        _view.DidNotReceive().DisplayMessage("Flashcard added successfully.");
    }

    [Test]
    public void ViewCards_WhenStackExists_MapsCardsToDtosAndDisplaysThem()
    {
        ConfigureSelectedStack("German", 17);

        var cards = new List<Flashcard>
        {
            new()
            {
                FlashcardId = 8,
                StackId = 17,
                Question = "Haus",
                Answer = "House"
            },
            new()
            {
                FlashcardId = 25,
                StackId = 17,
                Question = "Baum",
                Answer = "Tree"
            }
        };

        _flashcardsRepo.GetAllByStackId(17)
            .Returns(cards);

        _controller.ViewCards();

        _view.Received(1).DisplayFlashcards(
            Arg.Is<IReadOnlyList<FlashcardDTO>>(items =>
                items != null &&
                items.Count == 2 &&
                items[0].DisplayId == 1 &&
                items[0].Question == "Haus" &&
                items[0].Answer == "House" &&
                items[1].DisplayId == 2 &&
                items[1].Question == "Baum" &&
                items[1].Answer == "Tree"));

        _view.Received(1).WaitForInput();
    }

    [Test]
    public void ViewCards_WhenStackLookupThrows_DisplaysErrorAndDoesNotLoadCards()
    {
        SetCurrentStackName("German");

        _stacksRepo.GetStackByName("German")
            .Returns(_ => throw new InvalidOperationException("Database unavailable."));

        _controller.ViewCards();

        _view.Received(1).DisplayError("An error occurred while retrieving the stack: Database unavailable.");

        _flashcardsRepo.DidNotReceive().GetAllByStackId(Arg.Any<int>());
    }

    [Test]
    public void DeleteCard_WhenStackHasNoCards_DisplaysMessageAndDoesNotDelete()
    {
        ConfigureSelectedStack("German", 17);

        _flashcardsRepo.GetAllByStackId(17).Returns(Array.Empty<Flashcard>());

        _controller.DeleteCard();

        _view.Received(1).DisplayMessage("No flashcards found to delete.");

        _view.Received(1).WaitForInput();

        _flashcardsRepo.DidNotReceive().Delete(Arg.Any<int>());
    }

    [Test]
    public void DeleteCard_WhenCardIsSelected_DeletesRealFlashcardId()
    {
        ConfigureSelectedStack("German", 17);

        var cards = new List<Flashcard>
        {
            new()
            {
                FlashcardId = 8,
                StackId = 17,
                Question = "Haus",
                Answer = "House"
            },
            new()
            {
                FlashcardId = 25,
                StackId = 17,
                Question = "Baum",
                Answer = "Tree"
            }
        };

        _flashcardsRepo.GetAllByStackId(17).Returns(cards);

        _view.AskFlashcardIndex(2).Returns(2);

        _controller.DeleteCard();

        // display number 2 corresponds to real database ID 25
        _flashcardsRepo.Received(1).Delete(25);

        _view.Received(1).DisplayMessage("Flashcard deleted successfully.");
    }

    [Test]
    public void DeleteCard_WhenRepositoryThrows_DisplaysError()
    {
        ConfigureSelectedStack("German", 17);

        _flashcardsRepo.GetAllByStackId(17)
            .Returns(new List<Flashcard>
            {
                new()
                {
                    FlashcardId = 8,
                    StackId = 17,
                    Question = "Haus",
                    Answer = "House"
                }
            });

        _view.AskFlashcardIndex(1).Returns(1);

        _flashcardsRepo
            .When(repo => repo.Delete(8))
            .Do(_ => throw new InvalidOperationException(
                "Delete failed."));

        _controller.DeleteCard();

        _view.Received(1).DisplayError("Failed to delete flashcard: Delete failed.");
    }

    [Test]
    public void EditCard_WhenStackHasNoCards_DisplaysMessageAndDoesNotUpdate()
    {
        ConfigureSelectedStack("German", 17);

        _flashcardsRepo.GetAllByStackId(17).Returns(Array.Empty<Flashcard>());

        _controller.EditCard();

        _view.Received(1).DisplayMessage("No flashcards found to edit.");

        _view.Received(1).WaitForInput();

        _flashcardsRepo.DidNotReceive().Update(Arg.Any<Flashcard>());
    }
    [Test]
    public void EditCard_WhenCardIsSelected_UpdatesItsContent()
    {
        ConfigureSelectedStack("German", 17);

        var card = new Flashcard
        {
            FlashcardId = 8,
            StackId = 17,
            Question = "Old question",
            Answer = "Old answer"
        };

        _flashcardsRepo.GetAllByStackId(17).Returns(new List<Flashcard> { card });

        _view.AskFlashcardIndex(1).Returns(1);

        _view.AskFlashcardContent().Returns(("New question", "New answer"));

        _controller.EditCard();

        _flashcardsRepo.Received(1).Update(
            Arg.Is<Flashcard>(updated =>
                updated != null &&
                updated.FlashcardId == 8 &&
                updated.StackId == 17 &&
                updated.Question == "New question" &&
                updated.Answer == "New answer"));

        _view.Received(1).DisplayMessage("Flashcard updated successfully.");
    }

    [Test]
    public void EditCard_WhenRepositoryThrows_DisplaysError()
    {
        ConfigureSelectedStack("German", 17);

        _flashcardsRepo.GetAllByStackId(17)
            .Returns(new List<Flashcard>
            {
                new()
                {
                    FlashcardId = 8,
                    StackId = 17,
                    Question = "Old question",
                    Answer = "Old answer"
                }
            });

        _view.AskFlashcardIndex(1).Returns(1);

        _view.AskFlashcardContent().Returns(("New question", "New answer"));

        _flashcardsRepo
            .When(repo => repo.Update(Arg.Any<Flashcard>()))
            .Do(_ => throw new InvalidOperationException(
                "Update failed."));

        _controller.EditCard();

        _view.Received(1).DisplayError("Failed to update flashcard: Update failed.");
    }

    [Test]
    public void Run_WhenBackIsSelected_ReturnsWithoutCallingCrudMethods()
    {
        _view.ShowFlashcardsOption().Returns(FlashcardsOption.Back);

        _controller.Run();

        _view.Received(1).ShowFlashcardsOption();

        _flashcardsRepo.DidNotReceive().GetAllByStackId(Arg.Any<int>());

        _flashcardsRepo.DidNotReceive().Add(Arg.Any<Flashcard>());

        _flashcardsRepo.DidNotReceive().Update(Arg.Any<Flashcard>());

        _flashcardsRepo.DidNotReceive().Delete(Arg.Any<int>());
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
            card != null &&
            card.FlashcardId == 5 &&
            card.StackId == 10 &&
            card.Question == "Updated question" &&
            card.Answer == "Updated answer"));

        _view.Received(1).DisplayFlashcards(Arg.Any<IReadOnlyList<FlashcardDTO>>());
    }

    private void ConfigureSelectedStack(string name, int stackId)
    {
        SetCurrentStackName(name);

        _stacksRepo.GetStackByName(name)
            .Returns(new CardStack
            {
                StackId = stackId,
                Name = name
            });
    }

    private void SetCurrentStackName(string name)
    {
        FieldInfo? field = typeof(FlashcardController)
            .GetField(
                "_currentStackName",
                BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.That(
            field,
            Is.Not.Null,
            "The _currentStackName field was not found.");

        field!.SetValue(_controller, name);
    }
}
