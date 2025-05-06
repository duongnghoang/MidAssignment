using System.Security.Claims;
using Application.Services.Books;
using Contract.Dtos.Books.Requests;
using Contract.Dtos.Books.Responses;
using Contract.Shared;
using LibraryManagementSystem.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace WebAPI.Test;

[TestFixture]
public class BooksControllerTests
{
    private Mock<IBookService> _serviceMock;
    private BooksController _controller;

    [SetUp]
    public void Setup()
    {
        _serviceMock = new Mock<IBookService>();
        _controller = new BooksController(_serviceMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Test]
    public async Task GetListBooksFilterAsync_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new GetAllBooksFilterRequestDto
        {
            PageIndex = 1,
            PageSize = 10
        };
        var paginatedList = new PaginatedList<GetAllBooksResponseDto>(
            new List<GetAllBooksResponseDto>
            {
                new GetAllBooksResponseDto { Id = 1, Title = "Test Book" }
            }, 1, 1, 10);
        var result = Result.Success(paginatedList);

        _serviceMock.Setup(s => s.GetListBooksFilterAsync(request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetListBooksFilterAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.SameAs(result));
            Assert.That(((Result<PaginatedList<GetAllBooksResponseDto>>)okResult.Value).Value.Items,
                Has.Count.EqualTo(1));
            Assert.That(((Result<PaginatedList<GetAllBooksResponseDto>>)okResult.Value).Value?.Items[0].Title,
                Is.EqualTo("Test Book"));
        });
    }

    [Test]
    public async Task GetListBooksFilterAsync_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new GetAllBooksFilterRequestDto
        {
            PageIndex = 1,
            PageSize = 10
        };

        _serviceMock.Setup(s => s.GetListBooksFilterAsync(request))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.GetListBooksFilterAsync(request));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task AddBookAsync_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new AddBookRequestDto
        {
            Title = "New Book",
            Author = "Author",
            ISBN = "1234567890",
            PublicationDate = DateOnly.FromDateTime(DateTime.Today),
            Quantity = 10,
            CategoryId = 1
        };
        var result = Result.Success(1u);

        _serviceMock.Setup(s => s.AddBookAsync(request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.AddBookAsync(request);

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
    public async Task AddBookAsync_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        var request = new AddBookRequestDto
        {
            Title = "New Book",
            Author = "Author",
            ISBN = "1234567890",
            PublicationDate = DateOnly.FromDateTime(DateTime.Today),
            Quantity = 10,
            CategoryId = 1
        };
        var result = Result.Failure<uint>("A book with this ISBN already exists!");

        _serviceMock.Setup(s => s.AddBookAsync(request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.AddBookAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.SameAs(result));
            Assert.That(((Result<uint>)badRequestResult.Value).Error,
                Is.EqualTo("A book with this ISBN already exists!"));
        });
    }

    [Test]
    public async Task AddBookAsync_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        AddBookRequestDto request = null; // Invalid DTO
        _controller.ModelState.AddModelError("Request", "The Request field is required.");

        // Act
        var actionResult = await _controller.AddBookAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.InstanceOf<SerializableError>());
            var errors = (SerializableError)badRequestResult.Value;
            Assert.That(errors.ContainsKey("Request"), Is.True);
        });
    }

    [Test]
    public async Task AddBookAsync_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new AddBookRequestDto
        {
            Title = "New Book",
            Author = "Author",
            ISBN = "1234567890",
            PublicationDate = DateOnly.FromDateTime(DateTime.Today),
            Quantity = 10,
            CategoryId = 1
        };

        _serviceMock.Setup(s => s.AddBookAsync(request))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.AddBookAsync(request));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task UpdateBook_ValidRequest_ReturnsNoContentResult()
    {
        // Arrange
        const uint id = 1u;
        var request = new UpdateBookRequestDto
        {
            Title = "Updated Book",
            Author = "Updated Author",
            ISBN = "0987654321",
            PublicationDate = DateOnly.FromDateTime(DateTime.Today),
            Quantity = 15,
            Available = 12,
            CategoryId = 1
        };
        var result = Result.Success(true);

        _serviceMock.Setup(s => s.UpdateBookAsync(id, request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateBook(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<NoContentResult>());
            var noContentResult = actionResult as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
        });
    }

    [Test]
    public async Task UpdateBook_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        const uint id = 1u;
        var request = new UpdateBookRequestDto
        {
            Title = "Updated Book",
            Author = "Updated Author",
            ISBN = "0987654321",
            PublicationDate = DateOnly.FromDateTime(DateTime.Today),
            Quantity = 15,
            Available = 12,
            CategoryId = 1
        };
        var result = Result.Failure<bool>("Book not found!");

        _serviceMock.Setup(s => s.UpdateBookAsync(id, request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateBook(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.SameAs(result));
            Assert.That(((Result<bool>)badRequestResult.Value).Error, Is.EqualTo("Book not found!"));
        });
    }

    [Test]
    public async Task UpdateBook_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        const uint id = 1u;
        UpdateBookRequestDto request = null; // Invalid DTO
        _controller.ModelState.AddModelError("Request", "The Request field is required.");

        // Act
        var actionResult = await _controller.UpdateBook(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.InstanceOf<SerializableError>());
            var errors = (SerializableError)badRequestResult.Value;
            Assert.That(errors.ContainsKey("Request"), Is.True);
        });
    }

    [Test]
    public async Task UpdateBook_FuturePublicationDate_ReturnsBadRequest()
    {
        // Arrange
        const uint id = 1u;
        var request = new UpdateBookRequestDto
        {
            Title = "Updated Book",
            Author = "Updated Author",
            ISBN = "0987654321",
            PublicationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), // Future date
            Quantity = 15,
            Available = 12,
            CategoryId = 1
        };
        var result = Result.Failure<bool>("Publication date cannot be in the future.");

        _serviceMock.Setup(s => s.UpdateBookAsync(id, request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateBook(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.SameAs(result));
            Assert.That(((Result<bool>)badRequestResult.Value).Error,
                Is.EqualTo("Publication date cannot be in the future."));
        });
    }

    [Test]
    public async Task UpdateBook_AvailableExceedsQuantity_ReturnsBadRequest()
    {
        // Arrange
        const uint id = 1u;
        var request = new UpdateBookRequestDto
        {
            Title = "Updated Book",
            Author = "Updated Author",
            ISBN = "0987654321",
            PublicationDate = DateOnly.FromDateTime(DateTime.Today),
            Quantity = 10,
            Available = 15, // Available > Quantity
            CategoryId = 1
        };
        var result = Result.Failure<bool>("Available quantity cannot exceed total quantity.");

        _serviceMock.Setup(s => s.UpdateBookAsync(id, request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateBook(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.SameAs(result));
            Assert.That(((Result<bool>)badRequestResult.Value).Error,
                Is.EqualTo("Available quantity cannot exceed total quantity."));
        });
    }

    [Test]
    public async Task UpdateBook_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        const uint id = 1u;
        var request = new UpdateBookRequestDto
        {
            Title = "Updated Book",
            Author = "Updated Author",
            ISBN = "0987654321",
            PublicationDate = DateOnly.FromDateTime(DateTime.Today),
            Quantity = 15,
            Available = 12,
            CategoryId = 1
        };

        _serviceMock.Setup(s => s.UpdateBookAsync(id, request))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.UpdateBook(id, request));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task DeleteBook_ValidRequest_ReturnsNoContentResult()
    {
        // Arrange
        const uint id = 1u;
        var result = Result.Success(true);

        _serviceMock.Setup(s => s.DeleteBookAsync(id))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.DeleteBook(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<NoContentResult>());
            var noContentResult = actionResult as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
        });
    }

    [Test]
    public async Task DeleteBook_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        const uint id = 1u;
        var result = Result.Failure<bool>("Book not found!");

        _serviceMock.Setup(s => s.DeleteBookAsync(id))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.DeleteBook(id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.SameAs(result));
            Assert.That(((Result<bool>)badRequestResult.Value).Error, Is.EqualTo("Book not found!"));
        });
    }

    [Test]
    public async Task DeleteBook_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var id = 1u;

        _serviceMock.Setup(s => s.DeleteBookAsync(id))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.DeleteBook(id));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }
}