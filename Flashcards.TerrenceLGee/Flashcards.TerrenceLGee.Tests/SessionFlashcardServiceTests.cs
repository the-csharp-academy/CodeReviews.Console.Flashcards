using Flashcards.TerrenceLGee.Data.Interfaces;
using Flashcards.TerrenceLGee.DTOs.SessionFlashcardDTOs;
using Flashcards.TerrenceLGee.Models;
using Flashcards.TerrenceLGee.Services;
using Flashcards.TerrenceLGee.Services.Interfaces;
using Moq;

namespace Flashcards.TerrenceLGee.Tests;

public class SessionFlashcardServiceTests
{
    private readonly Mock<ISessionFlashcardRepository> _mockRepo;
    private readonly ISessionFlashcardService _sessionFlashcardService;

    public SessionFlashcardServiceTests()
    {
        _mockRepo = new Mock<ISessionFlashcardRepository>();
        _sessionFlashcardService = new SessionFlashcardService(_mockRepo.Object);
    }

    [Fact]
    public async Task AddSessionFlashcardAsync_ReturnsOne_WhenSuccessful()
    {
        const int expectedResult = 1;
        _mockRepo
            .Setup(r => r.AddSessionFlashcardAsync(It.IsAny<SessionFlashcard>()))
            .ReturnsAsync(expectedResult);

        var result = await _sessionFlashcardService.AddSessionFlashcardAsync(new SessionFlashcardDto());
        
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task AddSessionFlashcardAsync_ReturnsZero_WhenFailed()
    {
        const int expectedResult = 0;
        _mockRepo
            .Setup(r => r.AddSessionFlashcardAsync(It.IsAny<SessionFlashcard>()))
            .ReturnsAsync(expectedResult);

        var result = await _sessionFlashcardService.AddSessionFlashcardAsync(new SessionFlashcardDto());
        
        Assert.Equal(expectedResult, result);
    }
}