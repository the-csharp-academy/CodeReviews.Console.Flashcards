using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.DTOs.StudyStackDTOs;
using Flashcards.TerrenceLGee.Models;
using Flashcards.TerrenceLGee.Services;
using Flashcards.TerrenceLGee.Services.Interfaces;
using Moq;

namespace Flashcards.TerrenceLGee.Tests;

public class StudyStackServiceTests
{
    private readonly Mock<IStudyStackRepository> _mockRepo;
    private readonly IStudyStackService _stackService;
    private const int StackId = 1;

    public StudyStackServiceTests()
    {
        _mockRepo = new Mock<IStudyStackRepository>();
        _stackService = new StudyStackService(_mockRepo.Object);
    }

    [Fact]
    public async Task AddStackAsync_ShouldReturnOne_WhenSuccessful()
    {
        const int expectedResult = 1;
        _mockRepo
            .Setup(r => r.AddStackAsync(It.IsAny<StudyStack>()))
            .ReturnsAsync(expectedResult);

        var result = await _stackService.AddStackAsync(new CreateStudyStackDto());
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task AddStackAsync_ShouldReturnZero_WhenFailed()
    {
        const int expectedResult = 0;
        _mockRepo
            .Setup(r => r.AddStackAsync(It.IsAny<StudyStack>()))
            .ReturnsAsync(expectedResult);

        var result = await _stackService.AddStackAsync(new CreateStudyStackDto());
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task UpdateStackAsync_ShouldReturnOne_WhenSuccessful()
    {
        const int expectedResult = 1;
        _mockRepo
            .Setup(r => r.UpdateStackAsync(It.IsAny<StudyStack>()))
            .ReturnsAsync(expectedResult);

        var result = await _stackService.UpdateStackAsync(new UpdateStudyStackDto());
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task UpdateStackAsync_ShouldReturnZero_WhenFailed()
    {
        const int expectedResult = 0;
        _mockRepo
            .Setup(r => r.UpdateStackAsync(It.IsAny<StudyStack>()))
            .ReturnsAsync(expectedResult);

        var result = await _stackService.UpdateStackAsync(new UpdateStudyStackDto());
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task DeleteStackAsync_ShouldReturnOne_WhenSuccessful()
    {
        const int expectedResult = 1;
        _mockRepo
            .Setup(r => r.DeleteStackAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedResult);
        
        var result = await _stackService.DeleteStackAsync(StackId);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task DeleteStackAsync_ShouldReturnZero_WhenFailed()
    {
        const int expectedResult = 0;
        _mockRepo
            .Setup(r => r.DeleteStackAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _stackService.DeleteStackAsync(StackId);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task GetStackAsync_ShouldReturnStack_WhenStackExists()
    {
        var expectedResult = new StudyStack
        {
            Id = StackId,
            Subject = Subject.ComputerScience,
            Name = "Algorithms",
            Flashcards = [],
            StudySessions = []
        };

        _mockRepo
            .Setup(r => r.GetStackAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _stackService.GetStackAsync(StackId);

        Assert.NotNull(result);
        Assert.Equal(expectedResult.Subject, result.Subject);
        Assert.Equal(expectedResult.Name, result.Name);
    }

    [Fact]
    public async Task GetStackAsync_ShouldReturnNull_WhenStackDoesNotExist()
    {
        StudyStack? expectedResult = null;
        _mockRepo
            .Setup(r => r.GetStackAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _stackService.GetStackAsync(StackId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStacksAsync_ShouldReturnListOfStacks_WhenStacksAreAvailable()
    {
        var expectedResult = new List<StudyStack>
        {
            new()
            {
                Id = StackId,
                Subject = Subject.ComputerScience,
                Name = "Algorithms",
                Flashcards = [],
                StudySessions = []
            },
            new()
            {
                Id = StackId + 1,
                Subject = Subject.Music,
                Name = "Favorite Singers",
                Flashcards = [],
                StudySessions = []
            },
            new()
            {
                Id = StackId + 2,
                Subject = Subject.Law,
                Name = "Entertainment Law",
                Flashcards = [],
                StudySessions = []
            }
        };

        _mockRepo
            .Setup(r => r.GetStacksAsync())
            .ReturnsAsync(expectedResult);

        var result = await _stackService.GetStacksAsync();
        Assert.NotEmpty(result);
        Assert.Equal(expectedResult[0].Subject, result[0].Subject);
        Assert.Equal(expectedResult[2].Name, result[2].Name);
    }

    [Fact]
    public async Task GetStacksAsync_ShouldReturnEmptyList_WhenNoStacksAvailable()
    {
        List<StudyStack> expectedResult = [];
        _mockRepo
            .Setup(r => r.GetStacksAsync())
            .ReturnsAsync(expectedResult);

        var result = await _stackService.GetStacksAsync();
        
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStackCountAsync_ShouldReturnProperCountOfStacks_WhenStacksAreAvailable()
    {
        const int expectedResult = 3;
        _mockRepo
            .Setup(r => r.GetStackCountAsync())
            .ReturnsAsync(expectedResult);

        var result = await _stackService.GetStackCountAsync();
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task GetStackCountAsync_ShouldReturnZero_WhenStacksAreNotAvailable()
    {
        const int expectedResult = 0;
        _mockRepo
            .Setup(r => r.GetStackCountAsync())
            .ReturnsAsync(expectedResult);

        var result = await _stackService.GetStackCountAsync();
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task GetStackNameAndIdAsync_ShouldReturnDictionaryOfStackNamesAndIds_WhenStacksAreAvailable()
    {
        var expectedResult = new Dictionary<int, string>
        {
            { StackId, "Algorithms" },
            { StackId + 1, "Favorite Singers" },
            { StackId + 2, "Entertainment Law" },
        };

        _mockRepo
            .Setup(r => r.GetStackNameAndIdAsync())
            .ReturnsAsync(expectedResult);

        var result = await _stackService.GetStackNameAndIdAsync();
        
        Assert.NotEmpty(result);
        Assert.Equal(expectedResult.Keys.Count, result.Keys.Count);
        Assert.Equal(expectedResult[StackId], result[StackId]);
        Assert.Equal(expectedResult.Count, result.Count);
    }

    [Fact]
    public async Task GetStackNameAndIdAsync_ShouldReturnEmptyDictionary_WhenStacksAreNotAvailable()
    {
        Dictionary<int, string> expectedResult = [];

        _mockRepo
            .Setup(r => r.GetStackNameAndIdAsync())
            .ReturnsAsync(expectedResult);

        var result = await _stackService.GetStackNameAndIdAsync();
        
        Assert.Empty(result);
    }
}