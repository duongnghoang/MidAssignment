using Application.Services.Categories;
using Contract.Dtos.Categories.Requests;
using Contract.Dtos.Categories.Responses;
using Contract.Shared;
using LibraryManagementSystem.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace WebAPI.Test;

[TestFixture]
public class CategoriesControllerTests
{
    private Mock<ICategoryService> _serviceMock;
    private CategoriesController _controller;

    [SetUp]
    public void Setup()
    {
        _serviceMock = new Mock<ICategoryService>();
        _controller = new CategoriesController(_serviceMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Test]
    public async Task GetAllCategoriesAsync_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var categories = new List<GetCategoryResponseDto>
        {
            new GetCategoryResponseDto(Id: 1, Name: "Fiction")
        };
        var result = Result.Success(categories);

        _serviceMock.Setup(s => s.GetAllCategories())
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetAllCategoriesAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.SameAs(result));
            Assert.That(((Result<List<GetCategoryResponseDto>>)okResult.Value).Value, Has.Count.EqualTo(1));
            Assert.That(((Result<List<GetCategoryResponseDto>>)okResult.Value).Value[0].Name, Is.EqualTo("Fiction"));
        });
    }

    [Test]
    public async Task GetAllCategoriesAsync_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetAllCategories())
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.GetAllCategoriesAsync());
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task GetListCategoriesFilterAsync_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new GetAllCategoryFilterRequestDto
        {
            PageIndex = 1,
            PageSize = 10
        };
        var paginatedList = new PaginatedList<GetCategoryResponseDto>(
            new List<GetCategoryResponseDto>
            {
                new GetCategoryResponseDto(Id: 1, Name: "Fiction")
            }, 1, 1, 10);
        var result = Result.Success(paginatedList);

        _serviceMock.Setup(s => s.GetListCategoriesFilterAsync(request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetListCategoriesFilterAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.SameAs(result));
            Assert.That(((Result<PaginatedList<GetCategoryResponseDto>>)okResult.Value).Value.Items, Has.Count.EqualTo(1));
            Assert.That(((Result<PaginatedList<GetCategoryResponseDto>>)okResult.Value).Value.Items[0].Name, Is.EqualTo("Fiction"));
        });
    }

    [Test]
    public async Task GetListCategoriesFilterAsync_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new GetAllCategoryFilterRequestDto
        {
            PageIndex = 1,
            PageSize = 10
        };

        _serviceMock.Setup(s => s.GetListCategoriesFilterAsync(request))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.GetListCategoriesFilterAsync(request));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task AddNewCategoryAsync_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new AddCategoryRequestDto(Name: "Fiction");
        var result = Result.Success(1u);

        _serviceMock.Setup(s => s.AddNewCategoryAsync(request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.AddNewCategoryAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.SameAs(result));
            Assert.That(((Result<uint>)okResult.Value).Value, Is.EqualTo(1u));
        });
    }

    [Test]
    public async Task AddNewCategoryAsync_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        var request = new AddCategoryRequestDto(Name: "Fiction");
        var result = Result.Failure<uint>("Category name already exists");

        _serviceMock.Setup(s => s.AddNewCategoryAsync(request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.AddNewCategoryAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.SameAs(result));
            Assert.That(((Result<uint>)badRequestResult.Value).Error, Is.EqualTo("Category name already exists"));
        });
    }

    [Test]
    public async Task AddNewCategoryAsync_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        AddCategoryRequestDto request = null; // Invalid DTO
        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var actionResult = await _controller.AddNewCategoryAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.InstanceOf<SerializableError>());
            var errors = (SerializableError)badRequestResult.Value;
            Assert.That(errors.ContainsKey("Name"), Is.True);
        });
    }

    [Test]
    public async Task AddNewCategoryAsync_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new AddCategoryRequestDto(Name: "Fiction");

        _serviceMock.Setup(s => s.AddNewCategoryAsync(request))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.AddNewCategoryAsync(request));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task UpdateCategoryAsync_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        const uint id = 1u;
        var request = new UpdateCategoryRequestDto(Name: "Updated Fiction");
        var result = Result.Success(true);

        _serviceMock.Setup(s => s.UpdateCategoryAsync(id, request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateCategoryAsync(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<NoContentResult>());
            var noContentResult = actionResult as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
        });
    }

    [Test]
    public async Task UpdateCategoryAsync_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        const uint id = 1u;
        var request = new UpdateCategoryRequestDto(Name: "Updated Fiction");
        var result = Result.Failure<bool>("Category not found");

        _serviceMock.Setup(s => s.UpdateCategoryAsync(id, request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateCategoryAsync(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.SameAs(result));
            Assert.That(((Result<bool>)badRequestResult.Value).Error, Is.EqualTo("Category not found"));
        });
    }

    [Test]
    public async Task UpdateCategoryAsync_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        const uint id = 1u;
        UpdateCategoryRequestDto request = null; // Invalid DTO
        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var actionResult = await _controller.UpdateCategoryAsync(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.InstanceOf<SerializableError>());
            var errors = (SerializableError)badRequestResult.Value;
            Assert.That(errors.ContainsKey("Name"), Is.True);
        });
    }

    [Test]
    public async Task UpdateCategoryAsync_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        const uint id = 1u;
        var request = new UpdateCategoryRequestDto(Name: "Updated Fiction");

        _serviceMock.Setup(s => s.UpdateCategoryAsync(id, request))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.UpdateCategoryAsync(id, request));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task DeleteCategoryAsync_ValidRequest_ReturnsNoContentResult()
    {
        // Arrange
        var id = 1u;
        var result = Result.Success(true);

        _serviceMock.Setup(s => s.DeleteCategoryAsync(id))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.DeleteCategoryAsync(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<NoContentResult>());
            var noContentResult = actionResult as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
        });
    }

    [Test]
    public async Task DeleteCategoryAsync_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        const uint id = 1u;
        var result = Result.Failure<bool>("Category not found or has associated books");

        _serviceMock.Setup(s => s.DeleteCategoryAsync(id))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.DeleteCategoryAsync(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.SameAs(result));
            Assert.That(((Result<bool>)badRequestResult.Value).Error, Is.EqualTo("Category not found or has associated books"));
        });
    }

    [Test]
    public async Task DeleteCategoryAsync_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        const uint id = 1u;

        _serviceMock.Setup(s => s.DeleteCategoryAsync(id))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.DeleteCategoryAsync(id));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }
}