using System.Linq.Expressions;
using Application.Services.Users;
using Contract.Dtos.Users.Responses;
using Contract.Repositories;
using Contract.UnitOfWork;
using Domain.Entities;
using Moq;

namespace Application.Test;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private UserService _service;

    [SetUp]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _unitOfWorkMock.SetupGet(u => u.UserRepository).Returns(Mock.Of<IUserRepository>());

        _service = new UserService(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task GetUserByIdAsync_ValidId_ReturnsUserResponse()
    {
        // Arrange
        const uint id = 1u;
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            RoleId = 1,
            Role = new Role { Id = 1, Name = "Admin" }
        };
        var expectedResponse = new GetUserResponse
        {
            Username = "testuser",
            Email = "test@example.com",
            Role = "Admin"
        };

        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(id, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByIdAsync(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value!.Username, Is.EqualTo(expectedResponse.Username));
            Assert.That(result.Value.Email, Is.EqualTo(expectedResponse.Email));
            Assert.That(result.Value.Role, Is.EqualTo(expectedResponse.Role));
        });
    }

    [Test]
    public async Task GetUserByIdAsync_NonExistentId_ReturnsFailure()
    {
        // Arrange
        var id = 1u;

        _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(id, It.IsAny<Expression<Func<User, object>>>()))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _service.GetUserByIdAsync(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Null result"));
            Assert.That(result.Value, Is.Null);
        });
    }
}