using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.DTOs.StudySessionDTOs;
using Flashcards.TerrenceLGee.Models;
using Flashcards.TerrenceLGee.Services;
using Flashcards.TerrenceLGee.Services.Interfaces;
using Moq;

namespace Flashcards.TerrenceLGee.Tests;

public class StudySessionServiceTests
{
    private readonly Mock<IStudySessionRepository> _mockRepo;
    private readonly IStudySessionService _sessionService;
    private const int StackId = 1;
    private const int SessionId = 3;

    public StudySessionServiceTests()
    {
        _mockRepo = new Mock<IStudySessionRepository>();
        _sessionService = new StudySessionService(_mockRepo.Object);
    }

    [Fact]
    public async Task AddStudySessionAsync_ReturnsOne_WhenSuccessful()
    {
        const int expectedResult = 1;
        _mockRepo
            .Setup(r => r.AddStudySessionAsync(It.IsAny<StudySession>()))
            .ReturnsAsync(expectedResult);

        var result = await _sessionService.AddStudySessionAsync(new CreateStudySessionDto());
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task AddStudySessionAsync_ReturnsZero_WhenFailed()
    {
        const int expectedResult = 0;
        _mockRepo
            .Setup(r => r.AddStudySessionAsync(It.IsAny<StudySession>()))
            .ReturnsAsync(expectedResult);

        var result = await _sessionService.AddStudySessionAsync(new CreateStudySessionDto());
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task GetStudySessionAsync_ReturnsStudySession_WhenStackHasStudySession()
    {
        var expectedResult = new StudySession
        {
            Id = SessionId,
            StackId = StackId,
            TotalQuestions = 20,
            Correct = 20,
            Incorrect = 0,
            Score = 100,
            SessionDuration = new TimeSpan(0, 1, 55),
            SessionFlashcards = []
        };

        _mockRepo
            .Setup(r => r.GetStudySessionAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _sessionService.GetStudySessionAsync(StackId, SessionId);

        Assert.NotNull(result);
        Assert.Equal(expectedResult.TotalQuestions, result.TotalQuestions);
        Assert.Equal(expectedResult.SessionDuration, result.SessionDuration);
    }

    [Fact]
    public async Task GetStudySessionAsync_ReturnsNull_WhenStackDoesNotHaveStudySession()
    {
        StudySession? expectedResult = null;

        _mockRepo
            .Setup(r => r.GetStudySessionAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _sessionService.GetStudySessionAsync(StackId, SessionId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStudySessionsAsync_ReturnsListOfStudySessions_WhenStackHasStudySessions()
    {
        var expectedResult = new List<StudySession>
        {
            new()
            {
                Id = SessionId,
                StackId = StackId,
                TotalQuestions = 20,
                Correct = 20,
                Incorrect = 0,
                Score = 100,
                SessionDuration = new TimeSpan(0, 1, 55),
                SessionFlashcards = []
            },
            new()
            {
                Id = SessionId + 1,
                StackId = StackId,
                TotalQuestions = 20,
                Correct = 19,
                Incorrect = 1,
                Score = 95.1,
                SessionDuration = new TimeSpan(0, 2, 30),
                SessionFlashcards = []
            },
            new()
            {
                Id = SessionId + 2,
                StackId = StackId,
                TotalQuestions = 20,
                Correct = 10,
                Incorrect = 10,
                Score = 50,
                SessionDuration = new TimeSpan(0, 1, 14),
                SessionFlashcards = []
            }
        };

        _mockRepo
            .Setup(r => r.GetStudySessionsAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _sessionService.GetStudySessionsAsync(StackId);

        Assert.NotEmpty(result);
        Assert.Equal(expectedResult.Count, result.Count);
        Assert.Equal(expectedResult[2].SessionDuration, result[2].SessionDuration);
        Assert.Equal(expectedResult[1].TotalQuestions, result[1].TotalQuestions);
        Assert.Equal(expectedResult[0].Score, result[0].Score);
    }
    
    [Fact]
    public async Task GetStudySessionsAsync_ReturnsEmptyList_WhenStackDoesNotHaveStudySessions()
    {
        List<StudySession> expectedResult = [];

        _mockRepo
            .Setup(r => r.GetStudySessionsAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _sessionService.GetStudySessionsAsync(StackId);
        
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStudySessionCountAsync_ReturnsCountOfStudySessionForStack_WhenStackHasStudySessions()
    {
        const int expectedResult = 3;

        _mockRepo
            .Setup(r => r.GetStudySessionCountAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _sessionService.GetStudySessionCountAsync(StackId);
        
        Assert.Equal(expectedResult, result);
    }
    
    [Fact]
    public async Task GetStudySessionCountAsync_ReturnsZero_WhenStackDoesNotHaveStudySessions()
    {
        const int expectedResult = 0;
        
        _mockRepo
            .Setup(r => r.GetStudySessionCountAsync(It.IsAny<int>()))
            .ReturnsAsync(expectedResult);

        var result = await _sessionService.GetStudySessionCountAsync(StackId);
        
        Assert.Equal(expectedResult, result);
    }
}