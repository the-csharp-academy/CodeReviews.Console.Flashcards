using NSubstitute;

namespace CodeReviews.Console.Flashcards.Tests;

[TestFixture]
[Category("Unit")]
public sealed class StackControllerTests
{
    private IStacksView _view;
    private IStacksRepo _repository;
    private IStackController _controller;

    [SetUp]
    public void SetUp()
    {
        _view = Substitute.For<IStacksView>();
        _repository = Substitute.For<IStacksRepo>();

        _controller = new StackController(_view, _repository);
    }

    [Test]
    public void AddStack_WhenRepositorySucceeds_AddsStackAndDisplaysSuccess()
    {
        _view.AskForStackName().Returns("German");
        _controller.AddStack();
        _repository.Received(1).Add("German");
        _view.Received(1).DisplayMessage("Stack added successfully.");
        _view.DidNotReceive().DisplayError(Arg.Any<string>());
    }

    [Test]
    public void ViewStacks_MapsModelsToDtosAndDisplaysThem()
    {
        var stacks = new List<CardStack>
        {
            new() { StackId = 3, Name = "C#"},
            new() { StackId = 8, Name = "German"}
        };

        _repository.GetAll().Returns(stacks);
        _controller.ViewStacks();
        _view.Received(1).DisplayStacks(
            Arg.Is<IReadOnlyList<CardStackDTO>>(items =>
                items != null &&
                items.Count == 2 &&
                items[0].Name == "C#" &&
                items[1].Name == "German"));
    }

    [Test]
    public void DeleteStack_WhenStackExists_DeletesItByDatabaseId()
    {
        _view.AskForStackName().Returns("German");
        _repository.GetStackByName("German")
            .Returns(new CardStack
            {
                StackId = 12,
                Name = "German"
            });

        _controller.DeleteStack();
        _repository.Received(1).Delete(12);
    }

    [Test]
    public void DeleteStack_WhenStackDoesNotExist_DisplaysErrorAndDoesNotDelete()
    {
        _view.AskForStackName().Returns("Missing");
        _repository.GetStackByName("Missing").Returns((CardStack?)null);
        _controller.DeleteStack();
        _view.Received(1).DisplayError("Stack 'Missing' could not be found.");
        _repository.DidNotReceive().Delete(Arg.Any<int>());
        _view.DidNotReceive().DisplayMessage("Stack deleted successfully.");
    }

    [Test]
    public void EditStack_WhenStackExists_UpdatesItByDatabaseId()
    {
        // first for the current name, then for the new name.
        _view.AskForStackName().Returns("Old name", "New name");
        _repository.GetStackByName("Old name")
            .Returns(new CardStack
            {
                StackId = 7,
                Name = "Old name"
            });

        _controller.EditStack();
        _repository.Received(1).Update(7, "New name");
    }
    [Test]
    public void EditStack_WhenStackDoesNotExist_DisplaysErrorAndDoesNotUpdate()
    {
        _view.AskForStackName().Returns("Missing");
        _repository.GetStackByName("Missing").Returns((CardStack?)null);
        _controller.EditStack();
        _view.Received(1).DisplayError("Stack 'Missing' could not be found.");
        _view.Received(1).AskForStackName();
        _repository.DidNotReceive().Update(Arg.Any<int>(), Arg.Any<string>());
        _view.DidNotReceive().DisplayMessage("Stack updated successfully.");
    }

    [Test]
    public void ViewStacks_WhenRepositoryThrows_DisplaysErrorAndDoesNotDisplayStacks()
    {
        _repository
            .When(repository => repository.GetAll())
            .Do(_ => throw new Exception("Database unavailable"));

        _controller.ViewStacks();
        _view.Received(1).DisplayError("Could not retrieve stacks: Database unavailable");
        _view.DidNotReceive().DisplayStacks(Arg.Any<IReadOnlyList<CardStackDTO>>());
    }

    [Test]
    public void Run_WhenBackSelected_StopsImmediately()
    {
        _view.ShowStacksOption().Returns(StacksOption.Back);

        _controller.Run();

        _view.Received(1).ShowStacksOption();
        _repository.DidNotReceive().GetAll();
        _repository.DidNotReceive().Add(Arg.Any<string>());
        _repository.DidNotReceive().Delete(Arg.Any<int>());
        _repository.DidNotReceive().Update(Arg.Any<int>(), Arg.Any<string>());
    }

    [Test]
    public void Run_WhenBackSelected_ReturnsWithoutCallingCrudMethods()
    {
        _view.ShowStacksOption().Returns(StacksOption.Back);

        _controller.Run();

        _view.Received(1).ShowStacksOption();

        _view.DidNotReceive().AskForStackName();

        _repository.DidNotReceive().GetAll();
        _repository.DidNotReceive().Add(Arg.Any<string>());
        _repository.DidNotReceive().GetStackByName(Arg.Any<string>());
        _repository.DidNotReceive().Delete(Arg.Any<int>());
        _repository.DidNotReceive().Update(
            Arg.Any<int>(),
            Arg.Any<string>());
    }

    [Test]
    public void Run_WhenOptionIsUnknown_ThrowsArgumentOutOfRangeException()
    {
        _view.ShowStacksOption().Returns((StacksOption)999);
        Assert.Throws<ArgumentOutOfRangeException>(() => _controller.Run());
    }

    [Test]
    public void AddStack_WhenRepositoryThrows_DisplaysErrorAndNotSuccess()
    {
        _view.AskForStackName().Returns("German");
        var exception = new ArgumentNullException("name", "The stack name was null.");
        _repository.When(repository => repository.Add("German")).Do(_ => throw exception);
        _controller.AddStack();
        _view.Received(1).DisplayError($"Could not add stack: {exception.Message}");
        _view.DidNotReceive().DisplayMessage("Stack added successfully.");
    }

    [Test]
    public void DeleteStack_WhenDeleteThrows_DisplaysError()
    {
        _view.AskForStackName().Returns("German");

        var stack = new CardStack
        {
            StackId = 7,
            Name = "German"
        };

        _repository.GetStackByName("German").Returns(stack);

        _repository
            .When(repository => repository.Delete(stack.StackId))
            .Do(_ => throw new InvalidOperationException(
                "Database delete failed."));

        Assert.DoesNotThrow(() => _controller.DeleteStack());
        _view.Received(1).DisplayError(
            Arg.Is<string>(message => message != null &&
                message.Contains("Database delete failed.")));
    }

    [Test]
    public void EditStack_WhenStackLookupThrows_DisplaysErrorAndDoesNotUpdate()
    {
        _view.AskForStackName().Returns("German");
        _repository.GetStackByName("German").Returns(_ => throw new InvalidOperationException("Lookup failed."));

        _controller.EditStack();

        _view.Received(1).DisplayError("Could not retrieve stack: Lookup failed.");
        _view.Received(1).AskForStackName();
        _repository.DidNotReceive().Update(
                Arg.Any<int>(),
                Arg.Any<string>());
    }
}