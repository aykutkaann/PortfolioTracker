using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using PortfolioTracker.Application.DTOs.Auth;
using PortfolioTracker.Application.Interfaces;
using PortfolioTracker.Application.Services;
using PortfolioTracker.Domain.Entities;

namespace PortfolioTracker.UnitTests.Services;

public class AuthServiceTests
{

    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IJwtTokenService> _jwtMock = new();
    private readonly Mock<IConfiguration> _configMock = new();

    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_userRepoMock.Object, _configMock.Object, _jwtMock.Object);
    }


    [Fact] 
    public async Task RegisterAsync_ShouldReturnAuthResponse_WhenEmailIsNew()
    {
        
        _userRepoMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

       
        _jwtMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns(("fake-refresh-token", DateTime.UtcNow.AddDays(7)));

       
        _jwtMock
            .Setup(x => x.GenerateJwtToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");

        var request = new RegisterRequest("test@example.com", "Password123", "Test User");

        var result = await _sut.RegisterAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("fake-jwt-token");
        result.RefreshToken.Should().Be("fake-refresh-token");
        result.Email.Should().Be("test@example.com");

        _userRepoMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        _userRepoMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User("existing@test.com", "somehash", "Existing User"));

        var request = new RegisterRequest("existing@test.com", "Password123", "Test User");

        var act = () => _sut.RegisterAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Email*"); // * is wildcard — message contains "Email"
    }


    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenEmailNotFound()
    {
        _userRepoMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var request = new LoginRequest("nobody@test.com", "Password123");

        var act = () => _sut.LoginAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordIsWrong()
    {
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User("test@test.com", hashedPassword, "Test User");

        _userRepoMock
            .Setup(x => x.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var request = new LoginRequest("test@test.com", "WrongPassword");

        var act = () => _sut.LoginAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User("test@test.com", hashedPassword, "Test User");

        _userRepoMock
            .Setup(x => x.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _jwtMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns(("fake-refresh-token", DateTime.UtcNow.AddDays(7)));

        _jwtMock
            .Setup(x => x.GenerateJwtToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");

        var request = new LoginRequest("test@test.com", "CorrectPassword");

        var result = await _sut.LoginAsync(request, CancellationToken.None);

        
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("fake-jwt-token");
        result.Email.Should().Be("test@test.com");

        _userRepoMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task RefreshTokenAsync_ShouldThrow_WhenTokenIsInvalid()
    {
        _userRepoMock
            .Setup(x => x.GetByRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var request = new RefreshTokenRequest("invalid-token");

        var act = () => _sut.RefreshTokenAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*Invalid*");
    }
}
