using FluentAssertions;
using Moq;
using PortfolioTracker.Application.DTOs.Portfolio;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Application.Services;
using PortfolioTracker.Domain.Entities;

namespace PortfolioTracker.UnitTests.Services;

public class PortfolioServiceTests
{
    private readonly Mock<IPortfolioRepository> _portfolioRepoMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly PortfolioService _sut; 

    public PortfolioServiceTests()
    {
        _sut = new PortfolioService(_portfolioRepoMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnPortfolio_WhenUserIsAuthenticated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreatePortfolioRequest("My Crypto");

        _currentUserMock.Setup(x => x.UserId).Returns(userId);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        _portfolioRepoMock.Verify(x => x.AddAsync(It.Is<Portfolio>(p => p.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _currentUserMock.Setup(x => x.UserId).Returns((Guid?)null);
        var request = new CreatePortfolioRequest   ("Stealth Portfolio" );

        // Act
        Func<Task> act = async () => await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenPortfolioNotFound()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        _portfolioRepoMock.Setup(x => x.GetByIdAsync(portfolioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Portfolio?)null);

        // Act
        Func<Task> act = async () => await _sut.GetByIdAsync(portfolioId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("*not found*");

    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenUserDoesNotOwnPortfolio()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();

        var portfolio = new Portfolio(otherUserId, "Other Portfolio");


        _currentUserMock.Setup(x => x.UserId).Returns(currentUserId);
        _portfolioRepoMock.Setup(x => x.GetByIdAsync(portfolioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(portfolio);

        // Act
        Func<Task> act = async () => await _sut.GetByIdAsync(portfolioId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*You dont have access for this operation.*"); 
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenPortfolioNotFound()
    {
        // Arrange
        var portfolioId = Guid.NewGuid();
        _portfolioRepoMock.Setup(x => x.GetByIdAsync(portfolioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Portfolio?)null);

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync(portfolioId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("*not found*");

        _portfolioRepoMock.Verify(x => x.DeleteAsync(It.IsAny<Portfolio>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}