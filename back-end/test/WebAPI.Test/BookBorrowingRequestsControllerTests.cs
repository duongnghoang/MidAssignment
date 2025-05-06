using System.Security.Claims;
using Application.Services.BookBorrowingRequests;
using Contract.Dtos.BookBorrowRequestDetails.Requests;
using Contract.Dtos.BookBorrowRequests.Requests;
using Contract.Dtos.BookBorrowRequests.Responses;
using Contract.Shared;
using Domain.Enums;
using LibraryManagementSystem.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace WebAPI.Test;

[TestFixture]
public class BookBorrowingRequestsControllerTests
{
    private Mock<IBookBorrowingRequestService> _serviceMock;
    private BookBorrowingRequestsController _controller;

    [SetUp]
    public void Setup()
    {
        _serviceMock = new Mock<IBookBorrowingRequestService>();
        _controller = new BookBorrowingRequestsController(_serviceMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Test]
    public async Task GetListBookBorrowingRequestsFilterAsync_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new GetListBookBorrowingRequestFilterRequestDto
        {
            PageIndex = 1,
            PageSize = 10
        };
        var paginatedList = new PaginatedList<GetListBookBorrowingRequestFilterResponseDto>(
            new List<GetListBookBorrowingRequestFilterResponseDto>
            {
                new GetListBookBorrowingRequestFilterResponseDto { Id = 1 }
            }, 1, 1, 10);
        var result = Result.Success(paginatedList);

        _serviceMock.Setup(s => s.GetListBookBorrowingRequestAsync(request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetListBookBorrowingRequestsFilterAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.SameAs(result));
            Assert.That(((Result<PaginatedList<GetListBookBorrowingRequestFilterResponseDto>>)okResult.Value).Value.Items, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task GetListBookBorrowingRequestsFilterAsync_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new GetListBookBorrowingRequestFilterRequestDto
        {
            PageIndex = 1,
            PageSize = 10
        };

        _serviceMock.Setup(s => s.GetListBookBorrowingRequestAsync(request))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.GetListBookBorrowingRequestsFilterAsync(request));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task AddBookBorrowingRequestAsync_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new AddNewBookBorrowingRequestDto
        {
            RequestorId = 1,
            RequestDetails = new List<AddNewBookBorrowingRequestDetailDto>
            {
                new AddNewBookBorrowingRequestDetailDto { BookId = 1 }
            }
        };
        var result = Result.Success(1u);

        _serviceMock.Setup(s => s.AddNewBookBorrowingRequestAsync(request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.AddBookBorrowingRequestAsync(request);

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
    public async Task AddBookBorrowingRequestAsync_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        var request = new AddNewBookBorrowingRequestDto
        {
            RequestorId = 1,
            RequestDetails = new List<AddNewBookBorrowingRequestDetailDto>()
        };
        var result = Result.Failure<uint>("You must request between 1 and 5 books");

        _serviceMock.Setup(s => s.AddNewBookBorrowingRequestAsync(request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.AddBookBorrowingRequestAsync(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.SameAs(result));
            Assert.That(((Result<uint>)badRequestResult.Value).Error, Is.EqualTo("You must request between 1 and 5 books"));
        });
    }

    [Test]
    public async Task AddBookBorrowingRequestAsync_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new AddNewBookBorrowingRequestDto
        {
            RequestorId = 1,
            RequestDetails = new List<AddNewBookBorrowingRequestDetailDto>
            {
                new AddNewBookBorrowingRequestDetailDto { BookId = 1 }
            }
        };

        _serviceMock.Setup(s => s.AddNewBookBorrowingRequestAsync(request))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.AddBookBorrowingRequestAsync(request));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task GetBookBorrowingRequestByRequestorIdAsync_ValidId_ReturnsOkResult()
    {
        // Arrange
        const uint requestId = 1u;
        var result = Result.Success(new List<GetBookBorrowingRequestResponseDto>
        {
            new GetBookBorrowingRequestResponseDto { Id = 1 }
        });

        _serviceMock.Setup(s => s.GetListBookRequestByRequestor(requestId))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetBookBorrowingRequestByRequestorIdAsync(requestId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.SameAs(result));
            Assert.That(((Result<List<GetBookBorrowingRequestResponseDto>>)okResult.Value).Value, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task GetBookBorrowingRequestByRequestorIdAsync_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        const uint requestId = 1u;
        var result = Result.Failure<List<GetBookBorrowingRequestResponseDto>>("Requestor not found or not authorized");

        _serviceMock.Setup(s => s.GetListBookRequestByRequestor(requestId))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetBookBorrowingRequestByRequestorIdAsync(requestId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.SameAs(result));
            Assert.That(((Result<List<GetBookBorrowingRequestResponseDto>>)badRequestResult.Value).Error, Is.EqualTo("Requestor not found or not authorized"));
        });
    }

    [Test]
    public async Task GetBookBorrowingRequestByRequestorIdAsync_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var requestId = 1u;

        _serviceMock.Setup(s => s.GetListBookRequestByRequestor(requestId))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.GetBookBorrowingRequestByRequestorIdAsync(requestId));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task GetUserMonthRequest_ValidRequestorId_ReturnsOkResult()
    {
        // Arrange
        const uint requestorId = 1u;
        var result = Result.Success(2);

        _serviceMock.Setup(s => s.GetUserMonthRequest(requestorId))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetUserMonthRequest(requestorId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = actionResult as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.SameAs(result));
            Assert.That(((Result<int>)okResult.Value).Value, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GetUserMonthRequest_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        const uint requestorId = 1u;
        var result = Result.Failure<int>("Requestor not found or not authorized");

        _serviceMock.Setup(s => s.GetUserMonthRequest(requestorId))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetUserMonthRequest(requestorId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.SameAs(result));
            Assert.That(((Result<int>)badRequestResult.Value).Error, Is.EqualTo("Requestor not found or not authorized"));
        });
    }

    [Test]
    public async Task GetUserMonthRequest_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        const uint requestorId = 1u;

        _serviceMock.Setup(s => s.GetUserMonthRequest(requestorId))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.GetUserMonthRequest(requestorId));
        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task UpdateRequestStatus_ValidRequest_ReturnsNoContentResult()
    {
        // Arrange
        const uint id = 1u;
        var request = new UpdateBookStatusRequestDto
        {
            Status = Status.Approved,
            ApproverId = 2
        };
        var result = Result.Success(true);

        _serviceMock.Setup(s => s.UpdateRequestStatusAsync(id, request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateRequestStatus(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<NoContentResult>());
            var noContentResult = actionResult as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
        });
    }

    [Test]
    public async Task UpdateRequestStatus_ServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        const uint id = 1u;
        var request = new UpdateBookStatusRequestDto
        {
            Status = Status.Approved,
            ApproverId = 2
        };
        var result = Result.Failure<bool>("Request is not in Waiting status");

        _serviceMock.Setup(s => s.UpdateRequestStatusAsync(id, request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateRequestStatus(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = actionResult as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.SameAs(result));
            Assert.That(((Result<bool>)badRequestResult.Value).Error, Is.EqualTo("Request is not in Waiting status"));
        });
    }

    [Test]
    public async Task UpdateRequestStatus_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        const uint id = 1u;
        var request = new UpdateBookStatusRequestDto
        {
            Status = Status.Approved,
            ApproverId = 2
        };

        _serviceMock.Setup(s => s.UpdateRequestStatusAsync(id, request))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _controller.UpdateRequestStatus(id, request));

        Assert.That(exception.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task UpdateRequestStatus_WithSuperUserRole_ProcessesRequest()
    {
        // Arrange
        const uint id = 1u;
        var request = new UpdateBookStatusRequestDto
        {
            Status = Status.Approved,
            ApproverId = 2
        };
        var result = Result.Success(true);

        // Simulate SuperUser role
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "SuperUser") }, "mock"));
        _controller.ControllerContext.HttpContext.User = user;

        _serviceMock.Setup(s => s.UpdateRequestStatusAsync(id, request))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.UpdateRequestStatus(id, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.InstanceOf<NoContentResult>());
            var noContentResult = actionResult as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
        });
    }
}