using Application.Helpers;
using Application.Services.Authentication;
using Contract.Dtos.Authentication.Requests;
using Contract.Repositories;
using Contract.Services;
using Contract.UnitOfWork;
using Domain.Entities;
using Moq;

namespace Application.Test;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IJwtService> _jwtServiceMock;
    private AuthService _service;
    private List<User> _users;

    [SetUp]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _jwtServiceMock = new Mock<IJwtService>();
        _users = new List<User>();

        _unitOfWorkMock.SetupGet(u => u.UserRepository).Returns(Mock.Of<IUserRepository>());
        _service = new AuthService(_unitOfWorkMock.Object, _jwtServiceMock.Object);
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var request = new SignInRequest("testuser", "password123");
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = PasswordHelper.HashPassword("password123"),
            Email = "test@example.com",
            RoleId = 1
        };
        const string token = "jwt_token";

        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByUsername("testuser"))
            .ReturnsAsync(user);
        _jwtServiceMock.Setup(j => j.GenerateToken(user))
            .Returns(token);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(token));
        });
    }

    [Test]
    public async Task LoginAsync_NonExistentUser_ReturnsFailure()
    {
        // Arrange
        var request = new SignInRequest("testuser", "password123");

        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByUsername("testuser"))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Null user"));
        });
    }

    [Test]
    public async Task LoginAsync_InvalidPassword_ReturnsFailure()
    {
        // Arrange
        var request = new SignInRequest("testuser", "password123");

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = PasswordHelper.HashPassword("password12345"),
            Email = "test@example.com",
            RoleId = 1
        };

        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByUsername("testuser"))
            .ReturnsAsync(user);

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Invalid password"));
        });
    }

    [Test]
    public async Task RegisterAsync_ValidRequest_RegistersUser()
    {
        // Arrange
        var request = new SignUpRequest(Username: "newuser", Email: "newuser@example.com", Password: "password123",
            RoleId: 1);

        _unitOfWorkMock.Setup(u => u.UserRepository.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => _users.Add(u));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.RegisterAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_users, Has.Count.EqualTo(1));
            Assert.That(_users[0].Username, Is.EqualTo("newuser"));
            Assert.That(_users[0].Email, Is.EqualTo("newuser@example.com"));
            Assert.That(_users[0].RoleId, Is.EqualTo(1));
            Assert.That(PasswordHelper.VerifyPassword("password123", _users[0].PasswordHash), Is.True);
        });
    }

    [Test]
    public void RegisterAsync_DatabaseError_ThrowsException()
    {
        // Arrange
        var request = new SignUpRequest(Username: "newuser", Email: "newuser@example.com", Password: "password123",
            RoleId: 1);

        _unitOfWorkMock.Setup(u => u.UserRepository.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => _users.Add(u));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _service.RegisterAsync(request));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
        Assert.That(_users, Has.Count.EqualTo(1)); // User added but not saved
    }
}