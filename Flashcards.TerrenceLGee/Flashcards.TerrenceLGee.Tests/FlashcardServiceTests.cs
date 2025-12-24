using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.DTOs.FlashcardDTOs;
using Flashcards.TerrenceLGee.Models;
using Flashcards.TerrenceLGee.Services;
using Flashcards.TerrenceLGee.Services.Interfaces;
using Moq;

namespace Flashcards.TerrenceLGee.Tests;

public class FlashcardServiceTests
{
    private readonly Mock<IFlashcardRepository> _mockRepo;
    private readonly IFlashcardService _flashcardService;
    private const int StackId = 1;
    private const int FlashcardId = 3;
    private const int Position = 3;

    public FlashcardServiceTests()
    {
        _mockRepo = new Mock<IFlashcardRepository>();
        _flashcardService = new FlashcardService(_mockRepo.Object);
    }

    [Fact]
    public async Task AddFlashcardAsync_ReturnsOne_WhenSuccessful()
    {
        const int expectedResult = 1;
        _mockRepo
            .Setup(r => r.AddFlashcardAsync(It.IsAny<Flashcard>()))
            .ReturnsAsync(expectedResult);

        var result = await _flashcardService.AddFlashcardAsync(new CreateFlashcardDto());
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task AddFlashcardAsync_ReturnsZero_WhenFailed()
    {
        const int expectedResult = 0;
        _mockRepo
            .Setup(r => r.AddFlashcardAsync(It.IsAny<Flashcard>()))
            .ReturnsAsync(expectedResult);

        var result = await _flashcardService.AddFlashcardAsync(new CreateFlashcardDto());
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task UpdateFlashcardAsync_ReturnsOne_WhenSuccessful()
    {
        const int expectedResult = 1;
        _mockRepo
            .Setup(r => r.UpdateFlashcardAsync(It.IsAny<Flashcard>()))
            .ReturnsAsync(expectedResult);

        var result = await _flashcardService.UpdateFlashcardAsync(new UpdateFlashcardDto());
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task UpdateFlashcardAsync_ReturnsZero_WhenFailed()
    {
        const int expectedResult = 0;
        _mockRepo
            .Setup(r => r.UpdateFlashcardAsync(It.IsAny<Flashcard>()))
            .ReturnsAsync(expectedResult);

        var result = await _flashcardService.UpdateFlashcardAsync(new UpdateFlashcardDto());
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task DeleteFlashcardAsync_ReturnsOne_WhenSuccessful()
    {
        const int expectedResult = 1;
        _mockRepo
            .Setup(r => r.DeleteFlashcardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _flashcardService.DeleteFlashcardAsync(StackId, Position);
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task DeleteFlashcardAsync_ReturnsZero_WhenFailed()
    {
        const int expectedResult = 0;
        _mockRepo
            .Setup(r => r.DeleteFlashcardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _flashcardService.DeleteFlashcardAsync(StackId, Position);
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task GetFlashcardAsync_ReturnsFlashcard_WhenAvailable()
    {
        var expectedResult = new Flashcard
        {
            Id = FlashcardId,
            StackId = StackId,
            Question = "What is the most recent long term support release of .NET?",
            Answer = ".NET 10",
            Position = Position
        };

        _mockRepo
            .Setup(r => r.GetFlashcardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _flashcardService.GetFlashcardAsync(StackId, Position);

        Assert.NotNull(result);
        Assert.Equal(expectedResult.StackId, result.StackId);
        Assert.Equal(expectedResult.Question, result.Question);
    }

    [Fact]
    public async Task GetFlashcardAsync_ReturnsNull_WhenFlashcardDoesNotExist()
    {
        Flashcard? expectedResult = null;

        _mockRepo
            .Setup(r => r.GetFlashcardAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _flashcardService.GetFlashcardAsync(StackId, Position);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetFlashcardsAsync_ReturnsListOfFlashcards_WhenFlashcardsAreAvailable()
    {
        var expectedResult = new List<Flashcard>
        {
            new()
            {
                Id = FlashcardId + 1,
                StackId = StackId,
                Question = "What is the most recent long term support release of .NET?",
                Answer = ".NET 10",
                Position = Position + 1
            },
            new()
            {
                Id = FlashcardId + 1,
                StackId = StackId,
                Question = "What is the name of .NET Core's web-application framework?",
                Answer = "ASP.NET Core",
                Position = Position + 1
            },
            new()
            {
                Id = FlashcardId + 2,
                StackId = StackId,
                Question = "What software engineer authored Turbo Pascal and was the " +
                           "lead architect in the development of the C# programming " +
                           "language and core developer on TypeScript?",
                Answer = "Anders Hejlsberg",
                Position = Position + 2
            }
        };

        _mockRepo
            .Setup(r => r.GetFlashcardsAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _flashcardService.GetFlashcardsAsync(StackId);
        
        Assert.NotEmpty(result);
        Assert.Equal(expectedResult[0].StackId, result[0].StackId);
        Assert.Equal(expectedResult[1].Question, result[1].Question);
        Assert.Equal(expectedResult.Count, result.Count);
    }

    [Fact]
    public async Task GetFlashcardsAsync_ReturnsEmptyList_WhenFlashcardsAreNotAvailable()
    {
        List<Flashcard> expectedResult = [];

        _mockRepo
            .Setup(r => r.GetFlashcardsAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _flashcardService.GetFlashcardsAsync(StackId);
        
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetFlashcardCountAsync_ReturnsCountOfFlashcardsBelongingToStack_WhenStackHasFlashcards()
    {
        const int expectedResult = 3;

        _mockRepo
            .Setup(r => r.GetFlashcardCountAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _flashcardService.GetFlashcardCountAsync(StackId);
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task GetFlashcardCountAsync_ReturnsZero_WhenStackDoesNotHaveFlashcards()
    {
        const int expectedResult = 0;

        _mockRepo
            .Setup(r => r.GetFlashcardCountAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _flashcardService.GetFlashcardCountAsync(StackId);
        
        Assert.Equal(expectedResult, result);
    }
}